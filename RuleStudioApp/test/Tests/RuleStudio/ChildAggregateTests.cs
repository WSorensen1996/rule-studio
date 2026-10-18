using DataverseLogic.RuleStudio.Engine;
using Microsoft.Xrm.Sdk;
using SharedDomain.RuleStudio.Formats;
using static Tests.RuleStudio.EngineHarness;

namespace Tests.RuleStudio;

/// <summary>
/// Child paths (account → its contacts) with every aggregate. Dataverse runs the FetchXML; here the
/// rows it would return are given, and the query the engine sends is checked. XrmMockup does not
/// evaluate FetchXML aggregates, so the live suite covers the real numbers.
/// </summary>
public class ChildAggregateTests
{
    private const string ValueAlias = "rsvalue";

    [Theory]
    [InlineData("any", "CEO", "matched")]
    [InlineData("all", "CEO", "notMatched")]
    [InlineData("noneOf", "CFO", "matched")]
    [InlineData("noneOf", "CEO", "notMatched")]
    public void QuantifiersCountMatchingChildren(string aggregate, string title, string outcome)
    {
        var trace = Run(Child("jobtitle", "equals", title, aggregate), _ => Titles("CEO", "Sales", null));
        trace.Outcome.Should().Be(outcome);
        trace.Conditions!.Total.Should().Be(3);
    }

    [Theory]
    [InlineData("any", "notMatched")]
    [InlineData("all", "matched")]
    [InlineData("noneOf", "matched")]
    public void WithoutChildrenAllIsTrueAndTheTraceSaysZeroOfZero(string aggregate, string outcome)
    {
        var trace = Run(Child("jobtitle", "equals", "CEO", aggregate), _ => []);
        trace.Outcome.Should().Be(outcome);
        trace.Conditions!.Actual.Should().Be(0m);
        trace.Conditions.Total.Should().Be(0);
    }

    [Fact]
    public void ChildWithAnEmptyColumnCountsAsNotMatching() =>
        Run(Child("jobtitle", "isNotEmpty", null, "all"), _ => Titles("CEO", null)).Outcome.Should().Be("notMatched");

    [Theory]
    [InlineData("count", 3, "greaterThan", 2, "matched")]
    [InlineData("sum", 60, "equals", 60, "matched")]
    [InlineData("min", 5, "lessThan", 10, "matched")]
    [InlineData("max", 50, "greaterOrEqual", 51, "notMatched")]
    [InlineData("average", 20, "between", 20, "notMatched")]
    public void NumericAggregatesCompareTheResult(string aggregate, int result, string op, int expected, string outcome)
    {
        var value = op == "between" ? (object)new object[] { expected - 1, expected - 1 } : expected;
        Run(Child("numberofchildren", op, value, aggregate), _ => [AggregateRow(result)]).Outcome.Should().Be(outcome);
    }

    [Fact]
    public void SumOfMoneyIsComparedAsANumber() =>
        Run(Child("creditlimit", "greaterThan", 99.5, "sum"), _ => [AggregateRow(new Money(100m))]).Outcome.Should().Be("matched");

    [Fact]
    public void WithoutChildrenCountIsZeroAndTheOthersAreEmpty()
    {
        Run(Child("numberofchildren", "equals", 0, "count"), _ => [AggregateRow(null)]).Outcome.Should().Be("matched");
        Run(Child("numberofchildren", "isEmpty", null, "sum"), _ => [AggregateRow(null)]).Outcome.Should().Be("matched");
        Run(Child("numberofchildren", "equals", 0, "sum"), _ => [AggregateRow(null)]).Outcome.Should().Be("notMatched");
        Run(Child("numberofchildren", "isEmpty", null, "average"), _ => []).Outcome.Should().Be("matched");
    }

    [Fact]
    public void UnsavedRecordHasNoChildrenAndSendsNoQuery()
    {
        var queried = false;
        var record = new Entity(Table);
        var trace = Evaluate(record, Rule(Child("numberofchildren", "equals", 0, "count")), _ =>
        {
            queried = true;
            return [];
        }).Traces[0];
        trace.Outcome.Should().Be("matched");
        queried.Should().BeFalse();
    }

    [Fact]
    public void AggregateQueryFiltersOnTheParentAndAliasesTheResult()
    {
        string? fetch = null;
        var record = Record();
        Evaluate(record, Rule(Child("numberofchildren", "greaterThan", 1, "sum")), query =>
        {
            fetch = query;
            return [AggregateRow(2)];
        });
        fetch.Should().Contain("aggregate=\"true\"").And.Contain("<entity name=\"contact\">")
            .And.Contain($"<attribute name=\"numberofchildren\" alias=\"{ValueAlias}\" aggregate=\"sum\" />")
            .And.Contain($"<condition attribute=\"parentcustomerid\" operator=\"eq\" value=\"{record.Id}\" />");
    }

    [Fact]
    public void CountUsesCountColumnAndAverageUsesAvg()
    {
        var queries = new List<string>();
        Run(Child("numberofchildren", "isNotEmpty", null, "count"), q => Collect(queries, q));
        Run(Child("numberofchildren", "isNotEmpty", null, "average"), q => Collect(queries, q));
        queries[0].Should().Contain("aggregate=\"countcolumn\"");
        queries[1].Should().Contain("aggregate=\"avg\"");
    }

    [Fact]
    public void MoreChildRowsThanTheLimitFail()
    {
        var trace = Run(Child("jobtitle", "equals", "CEO", "any"), _ => Enumerable.Range(0, EngineLimits.MaxRows + 1).Select(_ => Contact("x")));
        trace.Outcome.Should().Be("failed");
        trace.Error!.Code.Should().Be("ROW_LIMIT_EXCEEDED");
    }

    [Fact]
    public void ChildPathWithoutAnAggregateFails() =>
        Run(Child("jobtitle", "equals", "CEO", "none"), _ => []).Error!.Code.Should().Be("CHILD_PATH_WITHOUT_AGGREGATE");

    [Fact]
    public void MoreThanFiveHopsFail()
    {
        var node = Child("jobtitle", "equals", "CEO", "any");
        for (var i = 0; i < 5; i++)
        {
            node.Path!.Hops.Add(new FieldPathHop { Kind = "parent", Attribute = "parentcustomerid", Table = "account" });
        }

        Run(node, _ => []).Error!.Code.Should().Be("HOP_LIMIT_EXCEEDED");
    }

    private static ExecutionTrace Run(CompiledNode condition, Func<string, IEnumerable<Entity>> rows) =>
        Evaluate(Record(), Rule(condition), rows).Traces[0];

    // A condition on a column of the account's contacts.
    private static CompiledNode Child(string column, string op, object? value, string aggregate)
    {
        var node = Condition(column, op, value, aggregate);
        node.Path!.Hops.Add(new FieldPathHop { Kind = "child", Attribute = "parentcustomerid", Table = "contact" });
        return node;
    }

    private static IEnumerable<Entity> Titles(params string?[] titles) => titles.Select(Contact);

    private static Entity Contact(string? title)
    {
        var contact = new Entity("contact", Guid.NewGuid());
        if (title != null)
        {
            contact["jobtitle"] = title;
        }

        return contact;
    }

    private static Entity AggregateRow(object? value) =>
        new("contact") { [ValueAlias] = new AliasedValue("contact", "numberofchildren", value) };

    private static IEnumerable<Entity> Collect(List<string> queries, string query)
    {
        queries.Add(query);
        return [AggregateRow(1)];
    }
}
