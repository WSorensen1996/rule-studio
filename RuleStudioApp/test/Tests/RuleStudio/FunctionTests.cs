using DataverseLogic.RuleStudio.Engine;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;
using static Tests.RuleStudio.EngineHarness;

namespace Tests.RuleStudio;

/// <summary>Custom functions: inputs, results, failures, the timeout, retries, test-mode mocking and Custom API parameter types.</summary>
public class FunctionTests
{
    [Fact]
    public void CallFunctionMapsFixedAndFieldPathInputs()
    {
        var (function, invoker) = Function(result: Ok(null));
        IDictionary<string, object?>? seen = null;
        invoker.Invoke(function, Arg.Do<IDictionary<string, object?>>(i => seen = i));
        var rule = Rule(Group("and"), Call(function, ("Text", Fixed("hi")), ("Count", Path("numberofemployees"))));
        rule.Functions.Add(function);

        var result = Run(new Setup { Invoker = invoker }, Record(("numberofemployees", 7)), rule);

        result.Traces[0].Outcome.Should().Be("matched");
        seen.Should().BeEquivalentTo(new Dictionary<string, object?>(StringComparer.Ordinal) { ["Text"] = "hi", ["Count"] = 7m });
        result.FunctionCalls.Single().Inputs["Count"].Should().Be(7m);
    }

    [Fact]
    public void FunctionValueIsWrittenBySetField()
    {
        var (function, invoker) = Function(result: Ok(12.5m));
        var rule = Rule(Group("and"), new CompiledAction { Id = Guid.NewGuid(), Type = "setField", TargetField = "decimalnumber", Value = new ValueSource { Type = "function", CustomFunctionId = function.Id } });
        rule.Functions.Add(function);

        Run(new Setup { Invoker = invoker }, Record(), rule).Changes["decimalnumber"].Should().Be(12.5m);
    }

    [Fact]
    public void ReportedFailureFailsTheRuleWithTheMessage()
    {
        var (function, invoker) = Function(result: new FunctionResult { Success = false, Message = "Rate service down" });
        var trace = RunOne(function, invoker);
        trace.Outcome.Should().Be("failed");
        trace.Error!.Code.Should().Be("FUNCTION_FAILED");
        trace.Error.Message.Should().Be("new_Rate failed: Rate service down");
    }

    [Fact]
    public void ThrowingFunctionFailsTheRule()
    {
        var (function, invoker) = Function();
        invoker.Invoke(Arg.Any<CompiledFunction>(), Arg.Any<IDictionary<string, object?>>()).Returns(_ => throw new InvalidPluginExecutionException("boom"));
        RunOne(function, invoker).Error!.Message.Should().Be("new_Rate failed: boom");
    }

    [Fact]
    public void CallLongerThanTheTimeoutFailsAndItsResultIsDiscarded()
    {
        var (function, invoker) = Function(slowMs: 60, result: Ok(5m));
        function.TimeoutMs = 10;
        var rule = Rule(Group("and"), new CompiledAction { Id = Guid.NewGuid(), Type = "setField", TargetField = "decimalnumber", Value = new ValueSource { Type = "function", CustomFunctionId = function.Id } });
        rule.Functions.Add(function);

        var result = Run(new Setup { Invoker = invoker }, Record(), rule);

        result.Traces[0].Error!.Code.Should().Be("FUNCTION_TIMEOUT");
        result.Traces[0].Error!.Message.Should().MatchRegex("new_Rate failed: took \\d+ ms; its timeout is 10 ms");
        result.FunctionCalls.Single().TimedOut.Should().BeTrue();
        result.Changes.Should().BeEmpty();
    }

    [Fact]
    public void WithoutATimeoutASlowCallSucceeds()
    {
        var (function, invoker) = Function(slowMs: 30, result: Ok(null));
        RunOne(function, invoker).Outcome.Should().Be("matched");
    }

    [Fact]
    public void MockedCallsNeverTimeOut()
    {
        var (function, invoker) = Function(slowMs: 30, result: Ok(null));
        function.TimeoutMs = 1;
        function.HasSideEffects = true;
        RunOne(function, invoker, simulation: true).Outcome.Should().Be("matched");
    }

    [Theory]
    [InlineData(true, 3)]
    [InlineData(false, 1)]
    public void OnlyFunctionsSafeToRetryAreRetried(bool safe, int calls)
    {
        var (function, invoker) = Function(result: new FunctionResult { Success = false, Message = "busy" });
        function.SafeToRetry = safe;
        var rule = Rule(Group("and"), Call(function));
        rule.Functions.Add(function);
        rule.Runtime.RetryCount = 2;

        var result = Run(new Setup { Invoker = invoker }, Record(), rule);

        invoker.ReceivedCalls().Count(c => c.GetMethodInfo().Name == nameof(IFunctionInvoker.Invoke)).Should().Be(calls);
        result.Attempts[rule.RuleId].Should().Be(calls);
    }

    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(false, true, true)]
    public void TestRunsMockSideEffectsAndOptionallyEverything(bool sideEffects, bool mockAll, bool mocked)
    {
        var (function, invoker) = Function(result: Ok(null));
        function.HasSideEffects = sideEffects;
        var rule = Rule(Group("and"), Call(function));
        rule.Functions.Add(function);

        var result = Run(new Setup { Invoker = invoker, Simulation = true, MockAll = mockAll }, Record(), rule);

        result.FunctionCalls.Single().Mocked.Should().Be(mocked);
        invoker.ReceivedCalls().Any(c => c.GetMethodInfo().Name == nameof(IFunctionInvoker.Invoke)).Should().Be(!mocked);
        if (mocked)
        {
            result.Traces[0].Test!.MockedFunctions.Should().Equal("new_Rate");
        }
    }

    [Fact]
    public void FunctionNotInTheSnapshotFails()
    {
        var (function, invoker) = Function();
        var rule = Rule(Group("and"), Call(function));
        Run(new Setup { Invoker = invoker }, Record(), rule).Traces[0].Error!.Code.Should().Be("FUNCTION_NOT_REGISTERED");
    }

    [Fact]
    public void CustomApiParametersGetTheirSchemaTypes()
    {
        var dao = Substitute.For<IDataverseAccessObject>();
        OrganizationRequest? sent = null;
        dao.Execute(Arg.Do<OrganizationRequest>(r => sent = r), Arg.Any<string>()).Returns(new OrganizationResponse());
        var id = Guid.NewGuid();
        var function = new CompiledFunction { ApiName = "new_All", InputSchema = new InputSchema() };
        foreach (var (name, type, table) in new[] { ("T", "text", (string?)null), ("N", "number", null), ("D", "decimal", null), ("M", "money", null), ("B", "boolean", null), ("Dt", "date", null), ("L", "lookup", "account"), ("X", "other", null) })
        {
            function.InputSchema.Parameters.Add(new FunctionParameter { Name = name, Type = type, Table = table });
        }

        new CustomApiInvoker(dao).Invoke(function, new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["T"] = 42m, ["N"] = 2.5m, ["D"] = "1.25", ["M"] = 3m, ["B"] = "true", ["Dt"] = "2026-01-01", ["L"] = id.ToString(), ["X"] = new List<object?> { "a", "b" }, ["Empty"] = string.Empty,
        });

        sent!["T"].Should().Be("42");
        sent["N"].Should().Be(3);
        sent["D"].Should().Be(1.25m);
        sent["M"].Should().BeEquivalentTo(new Money(3m));
        sent["B"].Should().Be(true);
        sent["Dt"].Should().Be(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        sent["L"].Should().BeEquivalentTo(new EntityReference("account", id));
        sent["X"].Should().Be("a, b");
        sent["Empty"].Should().BeNull();
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void ResponseWithoutSuccessCountsAsSuccess(bool? success, bool expected)
    {
        var dao = Substitute.For<IDataverseAccessObject>();
        var response = new OrganizationResponse();
        if (success != null)
        {
            response.Results["Success"] = success;
        }

        response.Results["value"] = 5;
        response.Results["Message"] = "note";
        dao.Execute(Arg.Any<OrganizationRequest>(), Arg.Any<string>()).Returns(response);

        var result = new CustomApiInvoker(dao).Invoke(new CompiledFunction { ApiName = "new_F" }, new Dictionary<string, object?>(StringComparer.Ordinal));

        result.Success.Should().Be(expected);
        result.Value.Should().Be(5m, "result names are matched ignoring case");
        result.Message.Should().Be("note");
    }

    [Fact]
    public void FindReadsAnEnabledRegisteredFunctionOnceAndKeepsIt()
    {
        var dao = Substitute.For<IDataverseAccessObject>();
        var row = new global::RuleStudio.SharedContext.rs_CustomFunction(Guid.NewGuid())
        {
            rs_Name = "Rate",
            rs_CustomApiName = "new_Rate",
            rs_Enabled = true,
            rs_HasSideEffects = true,
            rs_SafeToRetry = true,
            rs_TimeoutMs = 500,
            rs_InputSchema = "{\"version\":1,\"parameters\":[{\"name\":\"Currency\",\"type\":\"text\"}]}",
        };
        dao.RetrieveList<global::RuleStudio.SharedContext.rs_CustomFunction>(Arg.Any<Microsoft.Xrm.Sdk.Query.QueryExpression>(), Arg.Any<string>()).Returns([row]);
        var invoker = new CustomApiInvoker(dao);

        var found = invoker.Find("rate")!;
        invoker.Find("rate");

        found.ApiName.Should().Be("new_Rate");
        found.HasSideEffects.Should().BeTrue();
        found.TimeoutMs.Should().Be(500);
        found.InputSchema!.Parameters.Single().Name.Should().Be("Currency");
        dao.Received(1).RetrieveList<global::RuleStudio.SharedContext.rs_CustomFunction>(Arg.Any<Microsoft.Xrm.Sdk.Query.QueryExpression>(), Arg.Any<string>());
    }

    [Fact]
    public void FindIgnoresDisabledFunctionsAndBadSchemas()
    {
        var dao = Substitute.For<IDataverseAccessObject>();
        List<global::RuleStudio.SharedContext.rs_CustomFunction> off = [new(Guid.NewGuid()) { rs_Name = "Off", rs_CustomApiName = "new_Off", rs_Enabled = false }];
        List<global::RuleStudio.SharedContext.rs_CustomFunction> bad = [new(Guid.NewGuid()) { rs_Name = "Bad", rs_CustomApiName = "new_Bad", rs_InputSchema = "{not json" }];
        dao.RetrieveList<global::RuleStudio.SharedContext.rs_CustomFunction>(Arg.Any<Microsoft.Xrm.Sdk.Query.QueryExpression>(), Arg.Any<string>()).Returns(off, bad);
        var invoker = new CustomApiInvoker(dao);

        invoker.Find("Off").Should().BeNull();
        invoker.Find("Bad")!.InputSchema.Should().BeNull();
    }

    private static (CompiledFunction Function, IFunctionInvoker Invoker) Function(int slowMs = 0, FunctionResult? result = null)
    {
        var function = new CompiledFunction { Id = Guid.NewGuid(), Name = "Rate", ApiName = "new_Rate", SafeToRetry = true };
        var invoker = Substitute.For<IFunctionInvoker>();
        invoker.Invoke(Arg.Any<CompiledFunction>(), Arg.Any<IDictionary<string, object?>>()).Returns(_ =>
        {
            if (slowMs > 0)
            {
                Thread.Sleep(slowMs);
            }

            return result ?? Ok(null);
        });
        return (function, invoker);
    }

    private static FunctionResult Ok(object? value) => new() { Success = true, Value = value };

    private static ExecutionTrace RunOne(CompiledFunction function, IFunctionInvoker invoker, bool simulation = false)
    {
        var rule = Rule(Group("and"), Call(function));
        rule.Functions.Add(function);
        return Run(new Setup { Invoker = invoker, Simulation = simulation }, Record(), rule).Traces[0];
    }

    private static CompiledAction Call(CompiledFunction function, params (string Name, ValueSource Value)[] inputs)
    {
        var action = new CompiledAction { Id = Guid.NewGuid(), Type = "callFunction", Value = new ValueSource { Type = "function", CustomFunctionId = function.Id } };
        foreach (var (name, value) in inputs)
        {
            action.Inputs.Add(new CompiledInput { Name = name, Value = value });
        }

        return action;
    }

    private static ValueSource Path(string column) => new() { Type = "fieldPath", Path = new FieldPath { Table = Table, Column = column } };
}
