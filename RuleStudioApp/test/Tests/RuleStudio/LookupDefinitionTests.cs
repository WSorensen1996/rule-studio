using Newtonsoft.Json.Linq;
using RuleStudio.SharedContext;
using SharedDomain.RuleStudio.Formats;
using static Tests.RuleStudio.EngineHarness;

namespace Tests.RuleStudio;

/// <summary>Lookup definition criteria (equals, in, isEmpty, wildcards, unknown operators) and defaults, in XrmMockup.</summary>
public class LookupDefinitionTests : RuleStudioTestBase
{
    public LookupDefinitionTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Theory]
    [InlineData("Aarhus", 1)]
    [InlineData("Oslo", 3)]
    public void InMatchesAnyOfTheValues(string city, int expected)
    {
        TierRows("IN", ("Aarhus", 1m), ("Oslo", 3m));
        var lookup = Definition("in", new object[]
        {
            new { @operator = "equals", column = "accountnumber", value = Fixed("IN") },
            new { @operator = "in", column = "address1_city", values = new[] { City(), Fixed("Bergen") } },
        });
        ((decimal)TestLookup(lookup, new { address1_city = city })["lookup"]!["value"]!).Should().Be(expected);
    }

    [Fact]
    public void IsEmptyMatchesRowsWithoutAValue()
    {
        TierRows("EMPTY", ("Aarhus", 1m), (null, 2m));
        var lookup = Definition("isEmpty", new object[]
        {
            new { @operator = "equals", column = "accountnumber", value = Fixed("EMPTY") },
            new { @operator = "isEmpty", column = "address1_city" },
        });
        ((decimal)TestLookup(lookup, new { name = "x" })["lookup"]!["value"]!).Should().Be(2);
    }

    [Fact]
    public void EmptyInputMatchesOnlyRowsWithAnEmptyColumn()
    {
        TierRows("WILD", ("Aarhus", 1m), (null, 2m));
        var lookup = Definition("empty input", new object[]
        {
            new { @operator = "equals", column = "accountnumber", value = Fixed("WILD") },
            new { @operator = "equals", column = "address1_city", value = City() },
        });
        ((decimal)TestLookup(lookup, new { name = "no city" })["lookup"]!["value"]!).Should().Be(2);
    }

    [Fact]
    public void UnknownCriterionOperatorFails()
    {
        var lookup = Definition("unknown", new object[] { new { @operator = "like", column = "name", value = Fixed("x") } });
        TestLookup(lookup, new { name = "x" })["error"]!["code"]!.ToString().Should().Be("LOOKUP_INVALID");
    }

    [Fact]
    public void TextDefaultIsUsedWhenNothingMatches()
    {
        var lookup = Definition("default", new object[] { new { @operator = "equals", column = "accountnumber", value = Fixed("NONE") } }, rs_lookupdefinition_rs_nomatchpolicy.Usedefault, "\"fallback\"", "name");
        var result = TestLookup(lookup, new { name = "x" })["lookup"]!;
        result["value"]!.ToString().Should().Be("fallback");
        result["policy"]!.ToString().Should().Be("usedDefault");
    }

    [Fact]
    public void LookupMissingFromTheSnapshotFails()
    {
        var rule = Rule(Group("and"), new CompiledAction { Id = Guid.NewGuid(), Type = "setField", TargetField = "description", Value = new ValueSource { Type = "lookup", LookupDefinitionId = Guid.NewGuid() } });
        Run(new Setup(), Record(), rule).Traces[0].Error!.Code.Should().Be("LOOKUP_INVALID");
    }

    private static object Fixed(string value) => new { type = "fixed", value };

    private static object City() => new { type = "fieldPath", path = new { table = "account", hops = Array.Empty<object>(), column = "address1_city" } };

    private void TierRows(string tier, params (string? City, decimal Limit)[] rows)
    {
        foreach (var (city, limit) in rows)
        {
            NewAccount($"{tier} {city ?? "any"}", a => (a.AccountNumber, a.Address1_City, a.CreditLimit) = (tier, city, limit));
        }
    }

    private Guid Definition(string name, object[] criteria, rs_lookupdefinition_rs_nomatchpolicy noMatch = rs_lookupdefinition_rs_nomatchpolicy.Block, string? defaultValue = null, string returnColumn = "creditlimit") =>
        AdminDao.Create(new rs_LookupDefinition
        {
            rs_Name = "TEST – " + name,
            rs_LookupTable = "account",
            rs_ReturnColumn = returnColumn,
            rs_SortColumn = "creditlimit",
            rs_MatchCriteria = Json(new { version = 1, recordTable = "account", criteria }),
            rs_NoMatchPolicy = noMatch,
            rs_MultiMatchPolicy = rs_lookupdefinition_rs_multimatchpolicy.Firstbysort,
            rs_DefaultValue = defaultValue,
        });

    private JObject TestLookup(Guid lookupId, object sample) =>
        JObject.Parse((string)Admin.Execute(Api(
            "rs_SimulateRule",
            ("Table", "account"),
            ("SampleValues", Json(sample)),
            ("LookupDefinitionId", lookupId)))["Trace"]);
}
