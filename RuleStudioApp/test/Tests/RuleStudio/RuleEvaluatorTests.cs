using DataverseLogic.RuleStudio.Engine;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace Tests.RuleStudio;

/// <summary>In-memory tests of the rule evaluator: no Dataverse, root-table fields only.</summary>
public class RuleEvaluatorTests
{
    private const string Table = "account";

    [Theory]
    [InlineData("equals", 100, 100, true)]
    [InlineData("equals", 100, 99, false)]
    [InlineData("notEquals", 100, 99, true)]
    [InlineData("greaterThan", 100, 99, true)]
    [InlineData("greaterThan", 99, 100, false)]
    [InlineData("greaterOrEqual", 100, 100, true)]
    [InlineData("lessThan", 5, 6, true)]
    [InlineData("lessOrEqual", 7, 6, false)]
    public void NumberOperators(string op, int actual, int expected, bool matches)
    {
        var rule = Rule("r1", 1, Condition("revenue", op, expected));
        var result = Evaluate(Record(("revenue", new Money(actual))), rule);
        result.Traces[0].Outcome.Should().Be(matches ? "matched" : "notMatched");
    }

    [Theory]
    [InlineData("contains", "Contoso Ltd", "toso", true)]
    [InlineData("startsWith", "Contoso Ltd", "con", true)]
    [InlineData("startsWith", "Contoso Ltd", "ltd", false)]
    [InlineData("equals", "ABC", "abc", true)]
    public void TextOperators(string op, string actual, string expected, bool matches)
    {
        var result = Evaluate(Record(("name", actual)), Rule("r1", 1, Condition("name", op, expected)));
        result.Traces[0].Outcome.Should().Be(matches ? "matched" : "notMatched");
    }

    [Fact]
    public void EmptyInListAndBetween()
    {
        var record = Record(("name", string.Empty), ("numberofemployees", 50));
        Evaluate(record, Rule("r1", 1, Condition("name", "isEmpty", null))).Traces[0].Outcome.Should().Be("matched");
        Evaluate(record, Rule("r1", 1, Condition("numberofemployees", "inList", new object[] { 10, 50 }))).Traces[0].Outcome.Should().Be("matched");
        Evaluate(record, Rule("r1", 1, Condition("numberofemployees", "between", new object[] { 10, 49 }))).Traces[0].Outcome.Should().Be("notMatched");
        Evaluate(record, Rule("r1", 1, Condition("numberofemployees", "between", new object?[] { 10, null }))).Traces[0].Outcome.Should().Be("matched");
    }

    [Fact]
    public void AndOrGroupsShortCircuit()
    {
        var record = Record(("numberofemployees", 50));
        var and = Group("and", Condition("numberofemployees", "greaterThan", 100), Condition("numberofemployees", "lessThan", 100));
        var or = Group("or", Condition("numberofemployees", "greaterThan", 10), Condition("numberofemployees", "lessThan", 0));

        var andTrace = Evaluate(record, Rule("r1", 1, and)).Traces[0];
        andTrace.Outcome.Should().Be("notMatched");
        andTrace.Conditions!.Children[1].NotEvaluated.Should().BeTrue();

        var orTrace = Evaluate(record, Rule("r1", 1, or)).Traces[0];
        orTrace.Outcome.Should().Be("matched");
        orTrace.Conditions!.Children[1].NotEvaluated.Should().BeTrue();
        Evaluate(record, Rule("r1", 1, Group("and"))).Traces[0].Outcome.Should().Be("matched");
    }

    [Fact]
    public void PriorityThenKeyOrderAndLaterRulesSeeEarlierChanges()
    {
        var record = Record(("numberofemployees", 1));
        var second = Rule("b", 10, Condition("name", "equals", "set by a"));
        second.Then.Add(SetField("description", Fixed("b ran")));
        var first = Rule("a", 10, Group("and"));
        first.Then.Add(SetField("name", Fixed("set by a")));
        var last = Rule("z", 1, Group("and"));
        last.Then.Add(SetField("name", Fixed("set by z")));

        var result = Evaluate(record, second, first, last);

        result.Traces.Select(t => t.Rule!.RuleKey).Should().Equal("z", "a", "b");
        result.Traces[2].Outcome.Should().Be("matched");
        result.Changes["name"].Should().Be("set by a");
        result.Changes["description"].Should().Be("b ran");
    }

    [Fact]
    public void StopProcessingSkipsLaterRules()
    {
        var first = Rule("a", 1, Group("and"));
        first.Runtime.StopProcessing = true;
        var result = Evaluate(Record(), first, Rule("b", 2, Group("and")));
        result.Traces[1].Outcome.Should().Be("skipped");
        result.Traces[1].SkipReason.Should().Be("stoppedByEarlierRule");
    }

    [Fact]
    public void BlockSaveStopsTheChain()
    {
        var first = Rule("a", 1, Group("and"));
        first.Then.Add(new CompiledAction { Id = Guid.NewGuid(), Type = "blockSave", Message = "No." });
        var result = Evaluate(Record(), first, Rule("b", 2, Group("and")));
        result.BlockMessage.Should().Be("No.");
        result.Traces[0].Outcome.Should().Be("blocked");
        result.Traces[1].Outcome.Should().Be("skipped");
    }

    [Fact]
    public void ExpressionUsesFieldsAsParameters()
    {
        var rule = Rule("a", 1, Group("and"));
        rule.Then.Add(SetField("revenue", new ValueSource { Type = "expression", Expression = "[numberofemployees] * 2 + 1" }));
        rule.Then.Add(SetField("description", new ValueSource { Type = "expression", Expression = "if([numberofemployees] > 10, 'big', 'small')" }));
        var result = Evaluate(Record(("numberofemployees", 20)), rule);
        result.Traces[0].Error.Should().BeNull();
        result.Changes["revenue"].Should().Be(41m);
        result.Changes["description"].Should().Be("big");
    }

    [Fact]
    public void FailedExpressionUnderBlockPolicyBlocksBeforeSave()
    {
        var rule = Rule("a", 1, Group("and"));
        rule.Runtime.FailurePolicy = "block";
        rule.Then.Add(SetField("revenue", new ValueSource { Type = "expression", Expression = "[numberofemployees] +" }));
        var result = Evaluate(Record(("numberofemployees", 20)), rule);
        result.Traces[0].Outcome.Should().Be("failed");
        result.Traces[0].Error!.Code.Should().Be(EngineCodes.ExpressionFailed);
        result.BlockMessage.Should().NotBeNull();
        result.Changes.Should().BeEmpty();
    }

    [Fact]
    public void SideEffectFunctionsAreMockedInSimulation()
    {
        var function = new CompiledFunction { Id = Guid.NewGuid(), Name = "Notify", ApiName = "new_Notify", HasSideEffects = true };
        var rule = Rule("a", 1, Group("and"));
        rule.Functions.Add(function);
        rule.Then.Add(new CompiledAction { Id = Guid.NewGuid(), Type = "callFunction", Value = new ValueSource { Type = "function", CustomFunctionId = function.Id } });
        var invoker = Substitute.For<IFunctionInvoker>();

        var result = Evaluate(Record(), invoker, simulation: true, rule);

        invoker.DidNotReceiveWithAnyArgs().Invoke(default!, default!);
        result.Traces[0].Actions[0].Function!.Mocked.Should().BeTrue();
        result.Traces[0].Test!.MockedFunctions.Should().Contain("new_Notify");
    }

    private static EvaluationResult Evaluate(Entity record, params CompiledRule[] rules) =>
        Evaluate(record, Substitute.For<IFunctionInvoker>(), simulation: false, rules);

    private static EvaluationResult Evaluate(Entity record, IFunctionInvoker invoker, bool simulation, params CompiledRule[] rules)
    {
        var request = new EvaluationRequest(record, "update", "beforeSave") { Simulation = simulation };
        foreach (var rule in rules)
        {
            request.Rules.Add(rule);
        }

        return new RuleEvaluator(Substitute.For<IDataverseAccessObject>(), invoker).Evaluate(request);
    }

    private static Entity Record(params (string Column, object Value)[] values)
    {
        var record = new Entity(Table, Guid.NewGuid());
        foreach (var (column, value) in values)
        {
            record[column] = value;
        }

        return record;
    }

    private static CompiledRule Rule(string key, int priority, CompiledNode conditions)
    {
        var rule = new CompiledRule { RuleId = Guid.NewGuid(), RuleKey = key, Conditions = conditions };
        rule.Trigger.Table = Table;
        rule.Trigger.Event = "update";
        rule.Trigger.Stage = "beforeSave";
        rule.Runtime.Priority = priority;
        rule.Runtime.FailurePolicy = "logAndContinue";
        return rule;
    }

    private static CompiledNode Group(string op, params CompiledNode[] children)
    {
        var group = new CompiledNode { Type = "group", Id = Guid.NewGuid(), Operator = op };
        foreach (var child in children)
        {
            group.Children.Add(child);
        }

        return group;
    }

    private static CompiledNode Condition(string column, string op, object? value) => new()
    {
        Type = "condition",
        Id = Guid.NewGuid(),
        Path = new FieldPath { Table = Table, Column = column },
        Operator = op,
        Value = value == null ? null : Fixed(value),
    };

    private static ValueSource Fixed(object value) => new() { Type = "fixed", Value = value };

    private static CompiledAction SetField(string column, ValueSource value) =>
        new() { Id = Guid.NewGuid(), Type = "setField", TargetField = column, Value = value };
}
