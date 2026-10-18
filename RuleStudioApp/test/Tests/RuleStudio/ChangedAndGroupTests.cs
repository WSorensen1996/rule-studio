using Microsoft.Xrm.Sdk;
using static Tests.RuleStudio.EngineHarness;

namespace Tests.RuleStudio;

/// <summary>The Changed operator against the pre-image and saved columns, and nested AND/OR groups.</summary>
public class ChangedAndGroupTests
{
    [Fact]
    public void ChangedWhenTheSavedValueDiffersFromThePreImage() =>
        Changed(("name", "New"), ("name", "Old"), ["name"]).Should().Be("matched");

    [Fact]
    public void NotChangedWhenTheSavedValueIsTheSame() =>
        Changed(("name", "Same"), ("name", "Same"), ["name"]).Should().Be("notMatched");

    [Fact]
    public void NotChangedWhenTheColumnIsNotPartOfTheSave() =>
        Changed(("name", "New"), ("name", "Old"), ["description"]).Should().Be("notMatched");

    [Fact]
    public void ChangedWhenSavedAndThereIsNoPreImageValue() =>
        Changed(("name", "New"), ("description", "x"), ["name"]).Should().Be("matched");

    [Fact]
    public void ChoiceAndLookupChangesCompareValuesNotObjects()
    {
        var id = Guid.NewGuid();
        Changed(("industrycode", new OptionSetValue(1)), ("industrycode", new OptionSetValue(1)), ["industrycode"]).Should().Be("notMatched");
        Changed(("industrycode", new OptionSetValue(2)), ("industrycode", new OptionSetValue(1)), ["industrycode"]).Should().Be("matched");
        Changed(("parentaccountid", new EntityReference("account", id)), ("parentaccountid", new EntityReference("account", id)), ["parentaccountid"]).Should().Be("notMatched");
    }

    [Fact]
    public void MultiSelectInAnotherOrderIsNotAChange()
    {
        var before = new OptionSetValueCollection { new OptionSetValue(1), new OptionSetValue(2) };
        var after = new OptionSetValueCollection { new OptionSetValue(2), new OptionSetValue(1) };
        Changed(("tags", after), ("tags", before), ["tags"]).Should().Be("notMatched");
    }

    [Theory]
    [InlineData("create")]
    [InlineData("delete")]
    public void NeverChangedOutsideUpdate(string eventName)
    {
        var rule = Rule(Condition("name", "changed", null));
        Evaluate(Record(("name", "New")), rule, eventName, Record(("name", "Old")), new HashSet<string>(StringComparer.Ordinal) { "name" }).Traces[0].Outcome.Should().Be("notMatched");
    }

    [Fact]
    public void EmptyRootGroupAlwaysMatches() =>
        Evaluate(Record(), Rule(Group("and"))).Traces[0].Outcome.Should().Be("matched");

    [Fact]
    public void EmptyNestedGroupCountsAsTrue()
    {
        var rule = Rule(Group("and", Condition("name", "equals", "A"), Group("or")));
        Evaluate(Record(("name", "A")), rule).Traces[0].Outcome.Should().Be("matched");
    }

    [Theory]
    [InlineData("A", 5, true, "matched")]
    [InlineData("A", 50, false, "matched")]
    [InlineData("A", 50, true, "matched")]
    [InlineData("B", 5, true, "notMatched")]
    [InlineData("A", 5, false, "notMatched")]
    public void ThreeLevelsOfNesting(string name, int employees, bool onHold, string outcome)
    {
        // name = A AND (employees > 10 OR (onHold = true AND employees < 10))
        var rule = Rule(Group(
            "and",
            Condition("name", "equals", "A"),
            Group("or", Condition("numberofemployees", "greaterThan", 10), Group("and", Condition("creditonhold", "equals", true), Condition("numberofemployees", "lessThan", 10)))));
        Evaluate(Record(("name", name), ("numberofemployees", employees), ("creditonhold", onHold)), rule).Traces[0].Outcome.Should().Be(outcome);
    }

    [Fact]
    public void ShortCircuitedBranchesAreTracedAsNotEvaluated()
    {
        var rule = Rule(Group("or", Condition("name", "equals", "A"), Group("and", Condition("numberofemployees", "greaterThan", 1))));
        var root = Evaluate(Record(("name", "A")), rule).Traces[0].Conditions!;
        root.Children[0].Result.Should().BeTrue();
        root.Children[1].NotEvaluated.Should().BeTrue();
        root.Children[1].Children[0].NotEvaluated.Should().BeTrue();
    }

    private static string Changed((string Column, object Value) current, (string Column, object Value) previous, string[] saved)
    {
        var rule = Rule(Condition(current.Column, "changed", null));
        var trace = Evaluate(Record(current), rule, "update", Record(previous), new HashSet<string>(saved, StringComparer.Ordinal)).Traces[0];
        return trace.Outcome!;
    }
}
