using Newtonsoft.Json.Linq;
using RuleStudio.SharedContext;

namespace Tests.RuleStudio;

/// <summary>Rollback, restore and import edge cases in XrmMockup.</summary>
public class ReleaseFlowTests : RuleStudioTestBase
{
    public ReleaseFlowTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public void RollbackGoesBackToThePreviousVersionAndThenStopsTheRule()
    {
        var rule = Rows.NewRule("TEST.ROLL");
        var v1 = Version(rule, "one");
        var first = Publish(rule);
        var v2 = Version(rule, "two");
        var second = Publish(rule);

        var blocked = () => Rollback(first);
        blocked.Should().Throw<Exception>().WithMessage("*later release*");

        Rollback(second);
        Published(rule).Should().Be(v1);
        Status(v1).Should().Be(rs_ruleversion_rs_status.Published);
        Status(v2).Should().Be(rs_ruleversion_rs_status.Superseded);
        AdminDao.Retrieve<rs_Release>(second, r => r.rs_Status).rs_Status.Should().Be(rs_release_rs_status.Rolledback);

        Rollback(first);
        Published(rule).Should().BeNull("the first release had no earlier version to go back to");
    }

    [Fact]
    public void RestoreCopiesTheRowsAndReplacesTheDraftOnlyWhenAsked()
    {
        var rule = Rows.NewRule("TEST.RESTORE");
        var v1 = Version(rule, "one");
        Rows.Condition(v1, RuleRows.Path("account", "name"), rs_condition_rs_operator.Isnotempty);
        Publish(rule);
        var draft = Version(rule, "draft");

        var refused = () => Admin.Execute(Api("rs_RestoreVersion", ("RuleVersionId", v1)));
        refused.Should().Throw<Exception>().WithMessage("*already has a draft*");

        var restored = (Guid)Admin.Execute(Api("rs_RestoreVersion", ("RuleVersionId", v1), ("ReplaceDraft", true)))["RuleVersionId"];
        Status(draft).Should().Be(rs_ruleversion_rs_status.Discarded);
        AdminDao.Retrieve<rs_Rule>(rule, r => r.rs_DraftVersionId).rs_DraftVersionId!.Id.Should().Be(restored);
        var copy = AdminDao.Retrieve<rs_RuleVersion>(restored, v => v.rs_DerivedFromVersionId, v => v.rs_VersionNumber, v => v.rs_TargetTable);
        copy.rs_DerivedFromVersionId!.Id.Should().Be(v1);
        copy.rs_VersionNumber.Should().Be(3);
        copy.rs_TargetTable.Should().Be("account");
        Actions(restored).Should().ContainSingle(a => a.rs_Value == "\"one\"");
        var groups = AdminDao.RetrieveMultipleByIds<rs_ConditionGroup>([restored], "rs_ruleversionid");
        AdminDao.RetrieveMultipleByIds<rs_Condition>(groups.Select(g => g.Id), "rs_conditiongroupid").Should().ContainSingle();

        // The copy compiles to the same snapshot as the original apart from its ids.
        Publish(rule);
        var bundle = JObject.Parse((string)Admin.Execute(Api("rs_ExportRules"))["Bundle"]);
        bundle["rules"]![0]!["then"]![0]!["value"]!["value"]!.ToString().Should().Be("one");
    }

    [Fact]
    public void ImportReportsMissingAndChangedLookups()
    {
        var lookup = AdminDao.Create(new rs_LookupDefinition
        {
            rs_Name = "TEST – tier",
            rs_LookupTable = "account",
            rs_ReturnColumn = "creditlimit",
            rs_MatchCriteria = "{\"version\":1,\"recordTable\":\"account\",\"criteria\":[]}",
            rs_NoMatchPolicy = rs_lookupdefinition_rs_nomatchpolicy.Skip,
            rs_MultiMatchPolicy = rs_lookupdefinition_rs_multimatchpolicy.Firstbysort,
        });
        var rule = Rows.NewRule("TEST.USES_LOOKUP");
        var version = Rows.Draft(rule, "account", rs_event.Update, rs_stage.Beforesave);
        Rows.Action(version, rs_action_rs_actiontype.Setfield, a => (a.rs_TargetField, a.rs_ValueSourceType, a.rs_LookupDefinitionId) =
            ("creditlimit", rs_valuesourcetype.Lookup, new Microsoft.Xrm.Sdk.EntityReference(rs_LookupDefinition.EntityLogicalName, lookup)));
        Publish(rule);
        var bundle = JObject.Parse((string)Admin.Execute(Api("rs_ExportRules"))["Bundle"]);
        bundle["lookups"]!.Should().ContainSingle();

        var changed = (JObject)bundle.DeepClone();
        changed["lookups"]![0]!["returnColumn"] = "revenue";
        Import(changed, "check")["lookups"]![0]!["status"]!.ToString().Should().Be("update");

        var missing = (JObject)bundle.DeepClone();
        missing["lookups"] = new JArray();
        missing["rules"]![0]!["ruleKey"] = "TEST.NEW";
        missing["rules"]![0]!["then"]![0]!["value"]!["lookup"] = "TEST – no such lookup";
        var report = Import(missing, "apply");
        report["outcome"]!.ToString().Should().Be("blocked");
        report["issues"]!.Select(i => i["code"]!.ToString()).Should().Contain("IMPORT_LOOKUP_MISSING");

        Import(changed, "apply")["outcome"]!.ToString().Should().Be("applied");
        AdminDao.Retrieve<rs_LookupDefinition>(lookup, l => l.rs_ReturnColumn).rs_ReturnColumn.Should().Be("revenue");
    }

    [Fact]
    public void HashIgnoresEnvironmentIds()
    {
        // Two rules with identical logic in one environment differ only in ids and key, so with the key equal their hashes match.
        var a = Rows.NewRule("TEST.SAME_A");
        Version(a, "same");
        var b = Rows.NewRule("TEST.SAME_B");
        Version(b, "same");
        Publish(a, b);
        var rules = JObject.Parse((string)Admin.Execute(Api("rs_ExportRules"))["Bundle"])["rules"]!.Cast<JObject>().ToList();
        rules.Select(r => r["hash"]!.ToString()).Should().OnlyHaveUniqueItems("the key and name are part of the hash");
        foreach (var rule in rules)
        {
            rule["ruleKey"] = "TEST.SAME";
            rule["name"] = "same";
            rule.Remove("hash");
        }

        JToken.DeepEquals(rules[0], rules[1]).Should().BeTrue("no environment id is left in the portable rule");
    }

    [Fact]
    public void FailedValidationPublishesNothing()
    {
        var good = Rows.NewRule("TEST.GOOD");
        Version(good, "ok");
        var bad = Rows.NewRule("TEST.BAD");
        var badVersion = Rows.Draft(bad, "account", rs_event.Update, rs_stage.Beforesave);
        Rows.SetField(badVersion, "nosuchcolumn", "x");

        var response = Admin.Execute(Api("rs_PublishRelease", ("RuleIds", Json(new[] { good, bad })), ("Name", "TEST – all or nothing")));

        ((Guid)response["ReleaseId"]).Should().BeEmpty();
        JObject.Parse((string)response["ValidationResult"])["outcome"]!.ToString().Should().Be("failed");
        Published(good).Should().BeNull("a release goes live all or nothing");
        Published(bad).Should().BeNull();
        AdminDao.RetrieveList<rs_Release>(x => x.rs_ReleaseSet.Where(r => r.rs_Name == "TEST – all or nothing")).Should().BeEmpty();
    }

    [Fact]
    public void ReleaseOfSeveralRulesRollsBackTogether()
    {
        var a = Rows.NewRule("TEST.PAIR_A");
        var a1 = Version(a, "a1");
        var b = Rows.NewRule("TEST.PAIR_B");
        var b1 = Version(b, "b1");
        Publish(a, b);
        Version(a, "a2");
        Version(b, "b2");
        var second = Publish(a, b);

        Rollback(second);

        Published(a).Should().Be(a1);
        Published(b).Should().Be(b1);
    }

    [Fact]
    public void RollingBackARollbackIsRefused()
    {
        var rule = Rows.NewRule("TEST.ROLL_TWICE");
        Version(rule, "one");
        Publish(rule);
        Version(rule, "two");
        var second = Publish(rule);
        Rollback(second);
        var rollbackRelease = AdminDao.RetrieveList<rs_Release>(x => x.rs_ReleaseSet.Where(r => r.rs_RolledBackReleaseId!.Id == second)).Single().Id;

        var again = () => Rollback(second);
        again.Should().Throw<Exception>();
        var undo = () => Rollback(rollbackRelease);
        undo.Should().Throw<Exception>();
    }

    private JObject Import(JObject bundle, string mode) =>
        JObject.Parse((string)Admin.Execute(Api("rs_ImportRules", ("Bundle", bundle.ToString()), ("Mode", mode)))["Report"]);

    private Guid Version(Guid rule, string value)
    {
        var version = Rows.Draft(rule, "account", rs_event.Update, rs_stage.Beforesave, v => v.rs_FilteredColumns = "name");
        Rows.SetField(version, "description", value);
        return version;
    }

    private void Rollback(Guid releaseId) => Admin.Execute(Api("rs_RollbackRelease", ("ReleaseId", releaseId)));

    private Guid? Published(Guid rule) => AdminDao.Retrieve<rs_Rule>(rule, r => r.rs_PublishedVersionId).rs_PublishedVersionId?.Id;

    private rs_ruleversion_rs_status? Status(Guid version) => AdminDao.Retrieve<rs_RuleVersion>(version, v => v.rs_Status).rs_Status;

    private List<rs_Action> Actions(Guid version) => AdminDao.RetrieveMultipleByIds<rs_Action>([version], "rs_ruleversionid");
}
