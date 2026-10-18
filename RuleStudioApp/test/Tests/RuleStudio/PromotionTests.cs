using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuleStudio.SharedContext;

namespace Tests.RuleStudio;

/// <summary>rs_ExportRules and rs_ImportRules end to end in XrmMockup: publish, export, three-way check, apply.</summary>
public class PromotionTests : TestBase
{
    private const string Key = "TEST.SET_DESCRIPTION";

    public PromotionTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public void ExportedRuleIsUnchangedWhenImportedBack()
    {
        Publish(CreateDraft(Key, "first"));
        var bundle = Export();

        var rule = (JObject)bundle["rules"]![0]!;
        rule["ruleKey"]!.ToString().Should().Be(Key);
        rule["hash"]!.ToString().Should().HaveLength(64);
        rule.ToString().Should().NotContain("ruleVersionId").And.NotContain("\"id\"");

        var report = Import(bundle, "check");
        report["outcome"]!.ToString().Should().Be("clean");
        Status(report, Key).Should().Be("unchanged");
    }

    [Fact]
    public void ChangedRuleIsAppliedAndRoundTrips()
    {
        Publish(CreateDraft(Key, "first"));
        var bundle = Export();
        Import(bundle, "apply")["outcome"]!.ToString().Should().Be("applied");

        SetValue(bundle, "second");
        var check = Import(bundle, "check");
        Status(check, Key).Should().Be("update");

        var applied = Import(bundle, "apply");
        applied["outcome"]!.ToString().Should().Be("applied");
        applied["releaseId"].Should().NotBeNull();
        applied["issues"]!.Should().BeEmpty("the published version must hash like the incoming rule");

        var exported = (JObject)Export()["rules"]![0]!;
        exported["hash"]!.ToString().Should().Be(check["rules"]![0]!["incomingHash"]!.ToString());
        Import(Export(), "check")["rules"]![0]!["status"]!.ToString().Should().Be("unchanged");
    }

    [Fact]
    public void BothChangedIsAConflictAndApplyChangesNothing()
    {
        var ruleId = CreateDraft(Key, "first");
        Publish(ruleId);
        var bundle = Export();
        Import(bundle, "apply");

        // Edited here (new published version) and in the source (incoming value) since the last import.
        Publish(CreateDraftFor(ruleId, Key, "edited in target"));
        SetValue(bundle, "edited in source");

        var report = Import(bundle, "apply");
        report["outcome"]!.ToString().Should().Be("blocked");
        Status(report, Key).Should().Be("conflict");
        report["rules"]![0]!["current"].Should().NotBeNull();
        Codes(report).Should().Contain("IMPORT_CONFLICT");
        Export()["rules"]![0]!.ToString().Should().Contain("edited in target");
    }

    [Fact]
    public void OnlyTargetChangedKeepsTargetAndNewKeyIsCreated()
    {
        var ruleId = CreateDraft(Key, "first");
        Publish(ruleId);
        var bundle = Export();
        Import(bundle, "apply");
        Publish(CreateDraftFor(ruleId, Key, "edited in target"));

        var copy = (JObject)bundle["rules"]![0]!.DeepClone();
        copy["ruleKey"] = "TEST.NEW_RULE";
        ((JArray)bundle["rules"]!).Add(copy);

        var report = Import(bundle, "apply");
        report["outcome"]!.ToString().Should().Be("applied");
        Status(report, Key).Should().Be("keepTarget");
        Status(report, "TEST.NEW_RULE").Should().Be("create");
        Export()["rules"]!.Select(r => r["ruleKey"]!.ToString()).Should().Equal("TEST.NEW_RULE", Key);
    }

    [Fact]
    public void TargetOnlyRuleIsReportedAndMissingFunctionBlocks()
    {
        Publish(CreateDraft(Key, "first"));
        var empty = new JObject { ["version"] = 1, ["rules"] = new JArray(), ["lookups"] = new JArray() };
        Status(Import(empty, "check"), Key).Should().Be("targetOnly");

        var bundle = Export();
        var rule = (JObject)bundle["rules"]![0]!;
        rule["ruleKey"] = "TEST.CALLS_FUNCTION";
        rule["then"]![0]!["value"] = new JObject { ["type"] = "function", ["function"] = "rs_DoesNotExist" };
        var report = Import(bundle, "apply");
        report["outcome"]!.ToString().Should().Be("blocked");
        report["issues"]!.Select(i => i["code"]!.ToString()).Should().Contain("IMPORT_FUNCTION_MISSING");
    }

    [Fact]
    public void DuplicateOrMissingRuleKeyIsInvalid()
    {
        Publish(CreateDraft(Key, "first"));
        var bundle = Export();
        var copy = (JObject)bundle["rules"]![0]!.DeepClone();
        ((JArray)bundle["rules"]!).Add(copy);
        Codes(Import(bundle, "check")).Should().Contain("IMPORT_INVALID_RULE");

        copy["ruleKey"] = string.Empty;
        var report = Import(bundle, "apply");
        report["outcome"]!.ToString().Should().Be("blocked");
        Codes(report).Should().Contain("IMPORT_INVALID_RULE");
    }

    [Fact]
    public void UpdatingARuleWithALocalDraftWarnsThatTheDraftIsDiscarded()
    {
        var ruleId = CreateDraft(Key, "first");
        Publish(ruleId);
        var bundle = Export();
        Import(bundle, "apply");
        CreateDraftFor(ruleId, Key, "local draft");
        SetValue(bundle, "second");

        var report = Import(bundle, "apply");

        report["outcome"]!.ToString().Should().Be("applied");
        Codes(report).Should().Contain("IMPORT_DRAFT_DISCARDED");
        AdminDao.RetrieveList<rs_RuleVersion>(x => x.rs_RuleVersionSet.Where(v => v.rs_RuleId!.Id == ruleId && v.rs_Status == rs_ruleversion_rs_status.Discarded)).Should().ContainSingle();
    }

    private static IEnumerable<string> Codes(JObject report) => report["issues"]!.Select(i => i["code"]!.ToString());

    private static string Status(JObject report, string key) =>
        report["rules"]!.First(r => string.Equals(r["ruleKey"]!.ToString(), key, StringComparison.Ordinal))["status"]!.ToString();

    private static void SetValue(JObject bundle, string value) =>
        bundle["rules"]![0]!["then"]![0]!["value"]!["value"] = value;

    private Guid CreateDraft(string key, string value)
    {
        var ruleId = AdminDao.Create(new rs_Rule { rs_Name = "Set description", rs_RuleKey = key, rs_Enabled = true });
        CreateDraftFor(ruleId, key, value);
        return ruleId;
    }

    // A before-save Update rule on account: when name equals Contoso, set description.
    private Guid CreateDraftFor(Guid ruleId, string key, string value)
    {
        var versionId = AdminDao.Create(new rs_RuleVersion
        {
            rs_Name = key,
            rs_RuleId = new EntityReference(rs_Rule.EntityLogicalName, ruleId),
            rs_VersionNumber = 1,
            rs_Status = rs_ruleversion_rs_status.Draft,
            rs_TargetTable = "account",
            rs_Event = rs_event.Update,
            rs_Stage = rs_stage.Beforesave,
            rs_FilteredColumns = "name",
            rs_Priority = 100,
        });
        var group = AdminDao.Create(new rs_ConditionGroup
        {
            rs_Name = "Root",
            rs_LogicalOperator = rs_conditiongroup_rs_logicaloperator.AND,
            rs_RuleVersionId = new EntityReference(rs_RuleVersion.EntityLogicalName, versionId),
        });
        AdminDao.Create(new rs_Condition
        {
            rs_Name = "Name",
            rs_Order = 1,
            rs_ConditionGroupId = new EntityReference(rs_ConditionGroup.EntityLogicalName, group),
            rs_FieldPath = "{\"table\":\"account\",\"hops\":[],\"column\":\"name\"}",
            rs_Operator = rs_condition_rs_operator.Equals,
            rs_ValueSourceType = rs_valuesourcetype.Fixed,
            rs_Value = "\"Contoso\"",
        });
        AdminDao.Create(new rs_Action
        {
            rs_Name = "Description",
            rs_Order = 1,
            rs_RuleVersionId = new EntityReference(rs_RuleVersion.EntityLogicalName, versionId),
            rs_Branch = rs_action_rs_branch.Then,
            rs_ActionType = rs_action_rs_actiontype.Setfield,
            rs_TargetField = "description",
            rs_ValueSourceType = rs_valuesourcetype.Fixed,
            rs_Value = JsonConvert.SerializeObject(value),
        });
        AdminDao.Update(new rs_Rule(ruleId) { rs_DraftVersionId = new EntityReference(rs_RuleVersion.EntityLogicalName, versionId) });
        return ruleId;
    }

    private void Publish(Guid ruleId)
    {
        var response = Xrm.GetAdminService().Execute(new OrganizationRequest("rs_PublishRelease")
        {
            ["RuleIds"] = JsonConvert.SerializeObject(new[] { ruleId }),
            ["Name"] = "Test release",
        });
        ((Guid)response["ReleaseId"]).Should().NotBeEmpty(response["ValidationResult"]?.ToString());
    }

    private JObject Export() =>
        JObject.Parse((string)Xrm.GetAdminService().Execute(new OrganizationRequest("rs_ExportRules"))["Bundle"]);

    private JObject Import(JObject bundle, string mode) =>
        JObject.Parse((string)Xrm.GetAdminService().Execute(new OrganizationRequest("rs_ImportRules")
        {
            ["Bundle"] = bundle.ToString(Formatting.None),
            ["Mode"] = mode,
        })["Report"]);
}
