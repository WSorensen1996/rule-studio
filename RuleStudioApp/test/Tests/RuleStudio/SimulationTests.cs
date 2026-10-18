using Newtonsoft.Json.Linq;
using RuleStudio.SharedContext;

namespace Tests.RuleStudio;

/// <summary>
/// rs_SimulateRule in XrmMockup with real related records: parent paths, child aggregates, lookup
/// definitions with every policy, function value sources (the sample Custom APIs) and retries.
/// </summary>
public class SimulationTests : RuleStudioTestBase
{
    public SimulationTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    // count, sum, min, max and average are FetchXML aggregates, which XrmMockup does not evaluate:
    // they are verified against Dataverse by the live tests.
    [Theory]
    [InlineData(rs_condition_rs_aggregate.Any, "jobtitle", rs_condition_rs_operator.Equals, "CEO", true)]
    [InlineData(rs_condition_rs_aggregate.All, "jobtitle", rs_condition_rs_operator.Equals, "CEO", false)]
    [InlineData(rs_condition_rs_aggregate.Noneof, "jobtitle", rs_condition_rs_operator.Equals, "CFO", true)]
    public void ChildAggregates(rs_condition_rs_aggregate aggregate, string column, rs_condition_rs_operator op, object value, bool matches)
    {
        var account = NewAccount("Parent Co");
        NewContact("Ann", account, c => (c.JobTitle, c.NumberOfChildren) = ("CEO", 1));
        NewContact("Bob", account, c => (c.JobTitle, c.NumberOfChildren) = ("Sales", 2));
        NewContact("Cid", account, c => (c.JobTitle, c.NumberOfChildren) = ("Support", 3));
        var ruleId = Rows.NewRule("TEST.AGG");
        var version = Rows.Draft(ruleId, "account", rs_event.Update, rs_stage.Beforesave);
        Rows.Condition(version, RuleRows.Path("account", column, ("child", "parentcustomerid", "contact")), op, c => Fixed(c, value), aggregate);
        Rows.SetField(version, "description", "matched");

        Outcome(Simulate(ruleId, account)).Should().Be(matches ? "matched" : "notMatched");
    }

    [Fact]
    public void ParentPathReadsTheAccount()
    {
        var onHold = NewAccount("On hold", a => a.CreditOnHold = true);
        var fine = NewAccount("Fine", a => a.CreditOnHold = false);
        var ruleId = Rows.NewRule("TEST.R5");
        var version = Rows.Draft(ruleId, "contact", rs_event.Create, rs_stage.Beforesave);
        Rows.Condition(version, RuleRows.Path("contact", "creditonhold", ("parent", "parentcustomerid", "account")), rs_condition_rs_operator.Equals, c => Fixed(c, true));
        Rows.SetField(version, "description", "Parent on hold");

        Outcome(Simulate(ruleId, NewContact("Child", onHold))).Should().Be("matched");
        Outcome(Simulate(ruleId, NewContact("Other", fine))).Should().Be("notMatched");
        Outcome(Simulate(ruleId, NewContact("Orphan", null))).Should().Be("notMatched");
    }

    [Theory]
    [InlineData("Aarhus", rs_lookupdefinition_rs_multimatchpolicy.Mostspecific, 100, null)]
    [InlineData("Aarhus", rs_lookupdefinition_rs_multimatchpolicy.Firstbysort, 50, null)]
    [InlineData("Aarhus", rs_lookupdefinition_rs_multimatchpolicy.Error, null, "LOOKUP_MULTIPLE_MATCHES")]
    [InlineData("Oslo", rs_lookupdefinition_rs_multimatchpolicy.Error, 50, null)]
    public void LookupMultiMatchPolicies(string city, rs_lookupdefinition_rs_multimatchpolicy policy, int? expected, string? error)
    {
        TierRows();
        var lookup = Lookup(policy, rs_lookupdefinition_rs_nomatchpolicy.Block, "TIER");
        var result = TestLookup(lookup, new { address1_city = city });
        if (error == null)
        {
            result["error"].Should().BeNull();
            ((decimal)result["lookup"]!["value"]!).Should().Be(expected!.Value);
        }
        else
        {
            result["error"]!["code"]!.ToString().Should().Be(error);
        }
    }

    [Theory]
    [InlineData(rs_lookupdefinition_rs_nomatchpolicy.Block, "blocked")]
    [InlineData(rs_lookupdefinition_rs_nomatchpolicy.Usedefault, "usedDefault")]
    [InlineData(rs_lookupdefinition_rs_nomatchpolicy.Skip, "skipped")]
    public void LookupNoMatchPolicies(rs_lookupdefinition_rs_nomatchpolicy policy, string outcome)
    {
        TierRows();
        var lookup = Lookup(rs_lookupdefinition_rs_multimatchpolicy.Mostspecific, policy, "NO SUCH TIER");
        var result = TestLookup(lookup, new { address1_city = "Aarhus" });
        result["lookup"]!["policy"]!.ToString().Should().Be(outcome);
        if (policy == rs_lookupdefinition_rs_nomatchpolicy.Usedefault)
        {
            ((int)result["lookup"]!["value"]!).Should().Be(7);
        }
    }

    [Fact]
    public void LookupRangeUsesOpenBounds()
    {
        NewAccount("Tier small", a => (a.AccountNumber, a.NumberOfEmployees, a["sharesoutstanding"], a.CreditLimit) = ("RANGE", 0, 100, 1m));
        NewAccount("Tier large", a => (a.AccountNumber, a.NumberOfEmployees, a.CreditLimit) = ("RANGE", 100, 2m));
        var criteria = Json(new
        {
            version = 1,
            recordTable = "account",
            criteria = new object[]
            {
                new { @operator = "equals", column = "accountnumber", value = new { type = "fixed", value = "RANGE" } },
                new { @operator = "range", minColumn = "numberofemployees", maxColumn = "sharesoutstanding", value = new { type = "fieldPath", path = new { table = "account", hops = Array.Empty<object>(), column = "numberofemployees" } } },
            },
        });
        var lookup = AdminDao.Create(new rs_LookupDefinition
        {
            rs_Name = "TEST – range",
            rs_LookupTable = "account",
            rs_ReturnColumn = "creditlimit",
            rs_MatchCriteria = criteria,
            rs_NoMatchPolicy = rs_lookupdefinition_rs_nomatchpolicy.Block,
            rs_MultiMatchPolicy = rs_lookupdefinition_rs_multimatchpolicy.Error,
        });

        ((decimal)TestLookup(lookup, new { numberofemployees = 50 })["lookup"]!["value"]!).Should().Be(1);
        ((decimal)TestLookup(lookup, new { numberofemployees = 5000 })["lookup"]!["value"]!).Should().Be(2);
    }

    [Fact]
    public void FunctionValueSourceCallsTheCustomApi()
    {
        var function = Rows.Discount();
        var ruleId = Rows.NewRule("TEST.FN");
        var version = Rows.Draft(ruleId, "account", rs_event.Update, rs_stage.Beforesave);
        var action = Rows.Action(version, rs_action_rs_actiontype.Setfield, a =>
        {
            a.rs_TargetField = "revenue";
            a.rs_ValueSourceType = rs_valuesourcetype.Function;
            a.rs_CustomFunctionId = new Microsoft.Xrm.Sdk.EntityReference(rs_CustomFunction.EntityLogicalName, function);
        });
        Rows.Input(action, ownerIsAction: true, "Revenue", rs_valuesourcetype.Fieldpath, valuePath: RuleRows.Path("account", "revenue"));
        Rows.Input(action, ownerIsAction: true, "Employees", rs_valuesourcetype.Fieldpath, valuePath: RuleRows.Path("account", "numberofemployees"));

        var real = Simulate(ruleId, sample: new { revenue = 1000, numberofemployees = 150 }, mock: false);
        ((decimal)real[0]!["actions"]![0]!["after"]!).Should().Be(100m);
        real[0]!["actions"]![0]!["function"]!["mocked"]?.Value<bool>().Should().NotBe(true);

        var mocked = Simulate(ruleId, sample: new { revenue = 1000, numberofemployees = 150 });
        mocked[0]!["actions"]![0]!["function"]!["mocked"]!.Value<bool>().Should().BeTrue();
    }

    [Fact]
    public void FailingSafeFunctionIsRetriedAndLogAndContinueDoesNotBlock()
    {
        var function = Rows.Discount();
        var ruleId = Rows.NewRule("TEST.R4F");
        var version = Rows.Draft(ruleId, "account", rs_event.Update, rs_stage.Beforesave, v =>
        {
            v.rs_FailurePolicy = rs_ruleversion_rs_failurepolicy.Logandcontinue;
            v.rs_RetryCount = 2;
        });
        var action = Rows.Action(version, rs_action_rs_actiontype.Callfunction, a =>
        {
            a.rs_ValueSourceType = rs_valuesourcetype.Function;
            a.rs_CustomFunctionId = new Microsoft.Xrm.Sdk.EntityReference(rs_CustomFunction.EntityLogicalName, function);
        });
        Rows.Input(action, ownerIsAction: true, "Revenue", rs_valuesourcetype.Fixed, "-1");
        Rows.Input(action, ownerIsAction: true, "Employees", rs_valuesourcetype.Fixed, "1");

        var trace = Simulate(ruleId, sample: new { name = "x" }, mock: false)[0]!;
        trace["outcome"]!.ToString().Should().Be("failed");
        ((int)trace["error"]!["attempt"]!).Should().Be(3);
    }

    [Fact]
    public void OptionalFlagsDefaultToDraftsAndMocking()
    {
        var function = Rows.Discount();
        var ruleId = Rows.NewRule("TEST.DEFAULTS");
        var version = Rows.Draft(ruleId, "account", rs_event.Update, rs_stage.Beforesave);
        Rows.Action(version, rs_action_rs_actiontype.Callfunction, a => (a.rs_ValueSourceType, a.rs_CustomFunctionId) =
            (rs_valuesourcetype.Function, new Microsoft.Xrm.Sdk.EntityReference(rs_CustomFunction.EntityLogicalName, function)));

        // Only RuleIds: the draft is evaluated and functions are mocked (Dataverse passes absent Booleans as false).
        var traces = JArray.Parse((string)Admin.Execute(Api("rs_SimulateRule", ("RuleIds", Json(new[] { ruleId })), ("SampleValues", Json(new { name = "x" }))))["Trace"]);
        traces[0]!["rule"]!["draft"]!.Value<bool>().Should().BeTrue();
        traces[0]!["actions"]![0]!["function"]!["mocked"]!.Value<bool>().Should().BeTrue();
    }

    [Fact]
    public void LogMessageAndElseActions()
    {
        var ruleId = Rows.NewRule("TEST.LOG");
        var version = Rows.Draft(ruleId, "account", rs_event.Update, rs_stage.Beforesave);
        Rows.Condition(version, RuleRows.Path("account", "name"), rs_condition_rs_operator.Startswith, c => Fixed(c, "VIP"));
        Rows.Action(version, rs_action_rs_actiontype.Logmessage, a => (a.rs_Message, a.rs_MessageLevel) = ("VIP account", rs_messagelevel.Warning));
        Rows.SetField(version, "description", "regular", rs_action_rs_branch.Else);

        var vip = Simulate(ruleId, sample: new { name = "VIP Co" })[0]!;
        vip["actions"]![0]!["type"]!.ToString().Should().Be("logMessage");
        vip["actions"]![0]!["messageLevel"]!.ToString().Should().Be("warning");
        var regular = Simulate(ruleId, sample: new { name = "Plain Co" })[0]!;
        regular["branch"]!.ToString().Should().Be("else");
        regular["actions"]![0]!["after"]!.ToString().Should().Be("regular");
    }

    private static void Fixed(rs_Condition condition, object value)
    {
        condition.rs_ValueSourceType = rs_valuesourcetype.Fixed;
        condition.rs_Value = Json(value);
    }

    private static string Outcome(JArray traces) => traces[0]!["outcome"]!.ToString();

    // TIER rows: one for Aarhus (no wildcard), one with an empty city (wildcard).
    private void TierRows()
    {
        NewAccount("Tier Aarhus", a => (a.AccountNumber, a.Address1_City, a.CreditLimit) = ("TIER", "Aarhus", 100m));
        NewAccount("Tier any", a => (a.AccountNumber, a.CreditLimit) = ("TIER", 50m));
    }

    private Guid Lookup(rs_lookupdefinition_rs_multimatchpolicy multi, rs_lookupdefinition_rs_nomatchpolicy noMatch, string tier)
    {
        var criteria = Json(new
        {
            version = 1,
            recordTable = "account",
            criteria = new object[]
            {
                new { @operator = "equals", column = "accountnumber", value = new { type = "fixed", value = tier } },
                new { @operator = "equals", column = "address1_city", emptyMatchesAll = true, value = new { type = "fieldPath", path = new { table = "account", hops = Array.Empty<object>(), column = "address1_city" } } },
            },
        });
        return AdminDao.Create(new rs_LookupDefinition
        {
            rs_Name = "TEST – tier",
            rs_LookupTable = "account",
            rs_ReturnColumn = "creditlimit",
            rs_SortColumn = "creditlimit",
            rs_MatchCriteria = criteria,
            rs_NoMatchPolicy = noMatch,
            rs_MultiMatchPolicy = multi,
            rs_DefaultValue = "7",
        });
    }

    private JObject TestLookup(Guid lookupId, object sample) =>
        JObject.Parse((string)Admin.Execute(Api(
            "rs_SimulateRule",
            ("Table", "account"),
            ("SampleValues", Json(sample)),
            ("LookupDefinitionId", lookupId)))["Trace"]);
}
