using DataverseLogic.RuleStudio.Engine;
using Microsoft.Xrm.Sdk;
using SharedDomain.RuleStudio.Formats;
using static Tests.RuleStudio.EngineHarness;

namespace Tests.RuleStudio;

/// <summary>Field path values through parent lookups, and NCalc expressions with fields, built-ins and custom functions.</summary>
public class FieldPathAndExpressionTests
{
    [Fact]
    public void OneParentHopReadsTheParent()
    {
        var parent = Account("Parent Co");
        Read(Record(("parentaccountid", Ref(parent))), [parent], ParentPath(1, "name")).Should().Be("Parent Co");
    }

    [Fact]
    public void FiveParentHopsAreAllowed()
    {
        var chain = Chain(5);
        Read(Record(("parentaccountid", Ref(chain[0]))), chain, ParentPath(5, "name")).Should().Be("Level 5");
    }

    [Fact]
    public void SixHopsFail() =>
        Error(Record(("parentaccountid", Ref(Account("x")))), [], ParentPath(6, "name")).Should().Be("HOP_LIMIT_EXCEEDED");

    [Fact]
    public void EmptyLookupPartwayGivesAnEmptyValue()
    {
        var chain = Chain(2);
        chain[1].Attributes.Remove("parentaccountid");
        Read(Record(("parentaccountid", Ref(chain[0]))), chain, ParentPath(3, "name")).Should().BeNull();
        Read(Record(), chain, ParentPath(1, "name")).Should().BeNull();
    }

    [Fact]
    public void LookupToAnotherTableThanTheHopGivesAnEmptyValue() =>
        Read(Record(("parentaccountid", new EntityReference("contact", Guid.NewGuid()))), [], ParentPath(1, "name")).Should().BeNull();

    [Fact]
    public void MissingRelatedRecordGivesAnEmptyValue() =>
        Read(Record(("parentaccountid", new EntityReference("account", Guid.NewGuid()))), [], ParentPath(1, "name")).Should().BeNull();

    [Fact]
    public void PathWithoutAColumnFails() =>
        Error(Record(), [], new FieldPath { Table = Table, Column = string.Empty }).Should().Be("FIELD_NOT_FOUND");

    [Theory]
    [InlineData("Abs(-2)", 2)]
    [InlineData("Ceiling(1.2)", 2)]
    [InlineData("Floor(1.8)", 1)]
    [InlineData("Max(3, 7)", 7)]
    [InlineData("Min(3, 7)", 3)]
    [InlineData("Pow(2, 3)", 8)]
    [InlineData("Round(2.5, 0)", 3)]
    [InlineData("Round(-2.5, 0)", -3)]
    [InlineData("Round(2.345, 2)", 2.35)]
    [InlineData("Sign(-4)", -1)]
    [InlineData("Sqrt(16)", 4)]
    [InlineData("Truncate(2.9)", 2)]
    [InlineData("if(1 > 2, 10, 20)", 20)]
    [InlineData("[numberofemployees] * 2 + 1", 41)]
    [InlineData("[revenue] / 4", 2.5)]
    public void NumericExpressions(string expression, double expected) =>
        Number(Evaluate("decimalnumber", expression, ("numberofemployees", 20), ("revenue", new Money(10m)))).Should().BeApproximately((decimal)expected, 0.0000001m);

    [Theory]
    [InlineData("round(2.5, 0)")]
    [InlineData("ABS(-3)")]
    public void BuiltInsIgnoreCase(string expression) =>
        Evaluate("decimalnumber", expression).Should().NotBeNull();

    [Theory]
    [InlineData("in(2, 1, 2, 3)", true)]
    [InlineData("in(5, 1, 2, 3)", false)]
    [InlineData("[name] = 'Contoso'", true)]
    [InlineData("[creditonhold]", true)]
    public void BooleanExpressions(string expression, bool expected) =>
        Evaluate("creditonhold", expression, ("name", "Contoso"), ("creditonhold", true)).Should().Be(expected);

    [Fact]
    public void TextIsJoined() =>
        Evaluate("description", "[name] + ' Ltd'", ("name", "Contoso")).Should().Be("Contoso Ltd");

    [Fact]
    public void EmptyFieldMakesTheFormulaEmptySoSetFieldClearsTheColumn()
    {
        var result = Run(new Setup(), Record(), Rule(Group("and"), Expression("decimalnumber", "[numberofemployees] + 1")));
        result.Traces[0].Error.Should().BeNull();
        result.Changes["decimalnumber"].Should().BeNull();
    }

    [Fact]
    public void DivisionByZeroFails() =>
        EvaluateError("1 / 0", ("x", 1)).Should().BeOneOf("EXPRESSION_FAILED", null);

    [Fact]
    public void ParseErrorFails() =>
        EvaluateError("[numberofemployees] +").Should().Be("EXPRESSION_FAILED");

    [Fact]
    public void ParentFieldInAFormula()
    {
        var parent = Account("Parent Co");
        var rule = Rule(Group("and"), Expression("description", "[parentaccountid.name] + '!'"));
        Run(new Setup { Related = [parent] }, Record(("parentaccountid", Ref(parent))), rule).Changes["description"].Should().Be("Parent Co!");
    }

    [Fact]
    public void PolymorphicLookupNeedsTheTableInAFormula()
    {
        var rule = Rule(Group("and"), Expression("description", "[customerid.name]"));
        var trace = Run(new Setup(), Record(("customerid", new EntityReference("contact", Guid.NewGuid()))), rule).Traces[0];
        trace.Error!.Message.Should().Contain("must be a lookup to one table, or name the table as customerid:<table>");

        var named = Rule(Group("and"), Expression("description", "[customerid:contact.lastname]"));
        var contact = new Entity("contact", Guid.NewGuid()) { ["lastname"] = "Smith" };
        Run(new Setup { Related = [contact] }, Record(("customerid", new EntityReference("contact", contact.Id))), named).Changes["description"].Should().Be("Smith");
    }

    [Fact]
    public void CustomFunctionInAFormula()
    {
        var function = new CompiledFunction { Id = Guid.NewGuid(), Name = "Rate", ApiName = "new_Rate", InputSchema = new InputSchema() };
        function.InputSchema.Parameters.Add(new FunctionParameter { Name = "Currency", Type = "text" });
        var invoker = Substitute.For<IFunctionInvoker>();
        invoker.Invoke(function, Arg.Is<IDictionary<string, object?>>(i => (string?)i["Currency"] == "EUR")).Returns(new FunctionResult { Success = true, Value = 7.5m });
        var rule = Rule(Group("and"), Expression("decimalnumber", "new_Rate('EUR') * 2"));
        rule.Functions.Add(function);

        Run(new Setup { Invoker = invoker }, Record(), rule).Changes["decimalnumber"].Should().Be(15m);
    }

    [Fact]
    public void UnknownFunctionInAFormulaFails()
    {
        var rule = Rule(Group("and"), Expression("decimalnumber", "Nope(1)"));
        Run(new Setup(), Record(), rule).Traces[0].Error!.Code.Should().Be("FUNCTION_NOT_REGISTERED");
    }

    [Fact]
    public void FailingFunctionInAFormulaFailsTheRule()
    {
        var function = new CompiledFunction { Id = Guid.NewGuid(), Name = "Rate", ApiName = "new_Rate" };
        var invoker = Substitute.For<IFunctionInvoker>();
        invoker.Invoke(Arg.Any<CompiledFunction>(), Arg.Any<IDictionary<string, object?>>()).Returns(new FunctionResult { Success = false, Message = "down" });
        var rule = Rule(Group("and"), Expression("decimalnumber", "new_Rate()"));
        rule.Functions.Add(function);

        Run(new Setup { Invoker = invoker }, Record(), rule).Traces[0].Error!.Message.Should().Contain("new_Rate failed: down");
    }

    [Fact]
    public void EmptyExpressionFails() =>
        EvaluateError("   ").Should().Be("EXPRESSION_FAILED");

    private static Entity Account(string name) => new("account", Guid.NewGuid()) { ["name"] = name };

    private static EntityReference Ref(Entity record) => new(record.LogicalName, record.Id);

    // Accounts Level 1 … Level n, each the parent of the one before.
    private static List<Entity> Chain(int levels)
    {
        var chain = Enumerable.Range(1, levels).Select(i => Account($"Level {i}")).ToList();
        for (var i = 0; i < levels - 1; i++)
        {
            chain[i]["parentaccountid"] = Ref(chain[i + 1]);
        }

        return chain;
    }

    private static FieldPath ParentPath(int hops, string column)
    {
        var path = new FieldPath { Table = Table, Column = column };
        for (var i = 0; i < hops; i++)
        {
            path.Hops.Add(new FieldPathHop { Kind = "parent", Attribute = "parentaccountid", Table = "account" });
        }

        return path;
    }

    // The value a field path value source gives, written to description.
    private static object? Read(Entity record, IList<Entity> related, FieldPath path)
    {
        var rule = Rule(Group("and"), new CompiledAction { Id = Guid.NewGuid(), Type = "setField", TargetField = "description", Value = new ValueSource { Type = "fieldPath", Path = path } });
        var result = Run(new Setup { Related = related }, record, rule);
        result.Traces[0].Error.Should().BeNull();
        return result.Changes["description"];
    }

    private static string? Error(Entity record, IList<Entity> related, FieldPath path)
    {
        var rule = Rule(Group("and"), new CompiledAction { Id = Guid.NewGuid(), Type = "setField", TargetField = "description", Value = new ValueSource { Type = "fieldPath", Path = path } });
        return Run(new Setup { Related = related }, record, rule).Traces[0].Error?.Code;
    }

    private static CompiledAction Expression(string column, string text) =>
        new() { Id = Guid.NewGuid(), Type = "setField", TargetField = column, Value = new ValueSource { Type = "expression", Expression = text } };

    // The expression's value as written to the column.
    private static object? Evaluate(string column, string text, params (string Column, object? Value)[] values)
    {
        var result = Run(new Setup(), Record(values), Rule(Group("and"), Expression(column, text)));
        result.Traces[0].Error.Should().BeNull(result.Traces[0].Error?.Message);
        return result.Changes[column];
    }

    private static string? EvaluateError(string text, params (string Column, object? Value)[] values) =>
        Run(new Setup(), Record(values), Rule(Group("and"), Expression("decimalnumber", text))).Traces[0].Error?.Code;

    private static decimal Number(object? value) => Convert.ToDecimal(value, System.Globalization.CultureInfo.InvariantCulture);
}
