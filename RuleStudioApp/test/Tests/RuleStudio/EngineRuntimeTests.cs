extern alias engine;

using engine::RuleStudio.Engine;
using Microsoft.Xrm.Sdk;
using RuleStudio.SharedContext;

namespace Tests.RuleStudio;

/// <summary>
/// The engine plug-in on each trigger in XrmMockup: Create, Update and Delete before and after save,
/// filtered columns, the after-save update and its failure, the depth limit and disabled rules
/// (docs/engine.md, "What one execution does"). The platform's own re-triggering of after-save
/// updates is not simulated here; the live suite covers it.
/// </summary>
public class EngineRuntimeTests : RuleStudioTestBase
{
    private const int BeforeSave = 20;
    private const int AfterSave = 40;

    public EngineRuntimeTests(XrmMockupFixture fixture)
        : base(fixture)
    {
        RuleCatalog.Clear();
    }

    [Fact]
    public void UpdateBeforeSaveWritesIntoTheTarget()
    {
        var ruleId = SetRule("TEST.RT_UPD_BEFORE", rs_event.Update, rs_stage.Beforesave, "before");
        var id = Guid.NewGuid();
        var target = new Entity("account", id) { ["name"] = "TEST – Renamed" };

        Run("Update", BeforeSave, target, new Entity("account", id) { ["name"] = "TEST – Old" });

        target["description"].Should().Be("before");
        Logs(ruleId).Single().rs_Stage.Should().Be(rs_stage.Beforesave);
    }

    [Theory]
    [InlineData("Create", rs_event.Create)]
    [InlineData("Update", rs_event.Update)]
    public void AfterSaveUpdatesTheSavedRecord(string message, rs_event eventChoice)
    {
        ArgumentNullException.ThrowIfNull(message);
        var ruleId = SetRule("TEST.RT_AFTER_" + message.ToUpperInvariant(), eventChoice, rs_stage.Aftersave, "after");
        var account = NewAccount("After " + message);
        var target = new Entity("account", account) { ["name"] = "TEST – After " + message };

        Run(message, AfterSave, target, message == "Update" ? new Entity("account", account) { ["name"] = "TEST – x" } : null);

        target.Contains("description").Should().BeFalse("after save the target is already saved");
        AdminDao.Retrieve<Account>(account, a => a.Description).Description.Should().Be("after");
        var log = Logs(ruleId).Single();
        log.rs_Stage.Should().Be(rs_stage.Aftersave);
        log.rs_AdminFlag.Should().BeFalse();
    }

    [Fact]
    public void FailingAfterSaveUpdateIsLoggedAndFlaggedForAnAdmin()
    {
        var ruleId = SetRule("TEST.RT_AFTER_FAIL", rs_event.Update, rs_stage.Aftersave, "after", v => v.rs_LogLevel = rs_loglevel.Off);
        var missing = Guid.NewGuid();

        Run("Update", AfterSave, new Entity("account", missing) { ["name"] = "TEST – Gone" }, new Entity("account", missing) { ["name"] = "TEST – Gone" });

        // An after-save failure is logged whatever the level.
        var log = Logs(ruleId).Single();
        log.rs_Outcome.Should().Be(rs_executionlog_rs_outcome.Failed);
        log.rs_AdminFlag.Should().BeTrue();
        log.rs_ErrorMessage.Should().StartWith("APPLY_FAILED: The changes of the rule could not be saved");
    }

    [Fact]
    public void DeleteBeforeSaveCanBlockTheDelete()
    {
        var ruleId = Rows.NewRule("TEST.RT_DEL_BLOCK");
        var version = Rows.Draft(ruleId, "account", rs_event.Delete, rs_stage.Beforesave);
        Rows.Condition(version, RuleRows.Path("account", "creditonhold"), rs_condition_rs_operator.Equals, c => Fixed(c, true));
        Rows.Action(version, rs_action_rs_actiontype.Blocksave, a => a.rs_Message = "On-hold accounts cannot be deleted.");
        Publish(ruleId);
        var id = Guid.NewGuid();

        var delete = () => Run("Delete", BeforeSave, new EntityReference("account", id), new Entity("account", id) { ["creditonhold"] = true });

        delete.Should().Throw<InvalidPluginExecutionException>().WithMessage("On-hold accounts cannot be deleted.");
        Logs(ruleId).Should().BeEmpty("rows written before a block would be rolled back with the save, so none are written");
    }

    [Fact]
    public void DeleteRunsTheRuleButIgnoresFieldChanges()
    {
        var ruleId = SetRule("TEST.RT_DEL_SET", rs_event.Delete, rs_stage.Beforesave, "ignored");
        var id = Guid.NewGuid();
        var target = new EntityReference("account", id);

        Run("Delete", BeforeSave, target, new Entity("account", id) { ["name"] = "TEST – Deleted" });

        Logs(ruleId).Single().rs_Outcome.Should().Be(rs_executionlog_rs_outcome.Matched);
    }

    [Fact]
    public void FilteredRuleRunsOnlyWhenOneOfItsColumnsIsSaved()
    {
        var filtered = SetRule("TEST.RT_FILTERED", rs_event.Update, rs_stage.Beforesave, "filtered", v => v.rs_FilteredColumns = "name,accountnumber");
        var id = Guid.NewGuid();
        var pre = new Entity("account", id) { ["name"] = "TEST – Same" };

        var other = new Entity("account", id) { ["telephone1"] = "123" };
        Run("Update", BeforeSave, other, pre);
        other.Contains("description").Should().BeFalse();
        Logs(filtered).Should().BeEmpty("a rule left out by its filter is not run and not logged");

        var named = new Entity("account", id) { ["accountnumber"] = "A-1" };
        Run("Update", BeforeSave, named, pre);
        named["description"].Should().Be("filtered");
    }

    [Fact]
    public void UnfilteredRuleRunsOnAnyUpdate()
    {
        SetRule("TEST.RT_UNFILTERED", rs_event.Update, rs_stage.Beforesave, "any");
        var id = Guid.NewGuid();
        var target = new Entity("account", id) { ["telephone1"] = "123" };

        Run("Update", BeforeSave, target, new Entity("account", id));

        target["description"].Should().Be("any");
    }

    [Fact]
    public void RuleOnAnotherStageOrEventDoesNotRun()
    {
        SetRule("TEST.RT_CREATE_ONLY", rs_event.Create, rs_stage.Beforesave, "create");
        var id = Guid.NewGuid();
        var update = new Entity("account", id) { ["name"] = "TEST – x" };
        Run("Update", BeforeSave, update, new Entity("account", id));
        update.Contains("description").Should().BeFalse();

        var create = new Entity("account", Guid.NewGuid()) { ["name"] = "TEST – x" };
        Run("Create", AfterSave, create, null);
        create.Contains("description").Should().BeFalse();
    }

    [Fact]
    public void DisabledRuleDoesNotRun()
    {
        var ruleId = SetRule("TEST.RT_DISABLED", rs_event.Create, rs_stage.Beforesave, "x");
        AdminDao.Update(new rs_Rule(ruleId) { rs_Enabled = false });
        RuleCatalog.Clear();
        var target = new Entity("account", Guid.NewGuid()) { ["name"] = "TEST – x" };

        Run("Create", BeforeSave, target, null);

        target.Contains("description").Should().BeFalse();
        Logs(ruleId).Should().BeEmpty();
    }

    [Fact]
    public void DepthAtTheLimitStillRuns()
    {
        SetRule("TEST.RT_DEPTH", rs_event.Create, rs_stage.Beforesave, "deep");
        var target = new Entity("account", Guid.NewGuid()) { ["name"] = "TEST – deep" };

        Run("Create", BeforeSave, target, null, depth: 6);

        target["description"].Should().Be("deep");
    }

    [Fact]
    public void OtherMessagesAndStagesDoNothing()
    {
        SetRule("TEST.RT_ANY", rs_event.Update, rs_stage.Beforesave, "x");
        var target = new Entity("account", Guid.NewGuid()) { ["name"] = "TEST – x" };

        Run("Update", 10, target, null);
        Run("Assign", BeforeSave, target, null);

        target.Contains("description").Should().BeFalse();
    }

    private static void Fixed(rs_Condition condition, object value)
    {
        condition.rs_ValueSourceType = rs_valuesourcetype.Fixed;
        condition.rs_Value = Json(value);
    }

    // A published rule without conditions that sets description to the text.
    private Guid SetRule(string key, rs_event eventChoice, rs_stage stage, string text, Action<rs_RuleVersion>? settings = null)
    {
        var ruleId = Rows.NewRule(key);
        var version = Rows.Draft(ruleId, "account", eventChoice, stage, settings);
        Rows.SetField(version, "description", text);
        Publish(ruleId);
        return ruleId;
    }

    private List<rs_ExecutionLog> Logs(Guid ruleId) =>
        AdminDao.RetrieveList<rs_ExecutionLog>(x => x.rs_ExecutionLogSet.Where(l => l.rs_RuleId!.Id == ruleId));

    // One engine execution as the platform starts it, as the admin user.
    private void Run(string message, int stage, object target, Entity? preImage, int depth = 1)
    {
        var id = target is Entity e ? e.Id : ((EntityReference)target).Id;
        var table = target is Entity t ? t.LogicalName : ((EntityReference)target).LogicalName;
        var context = Substitute.For<IPluginExecutionContext>();
        context.MessageName.Returns(message);
        context.Stage.Returns(stage);
        context.Depth.Returns(depth);
        context.PrimaryEntityName.Returns(table);
        context.PrimaryEntityId.Returns(id);
        context.UserId.Returns(Xrm.AdminUser.Id);
        context.InitiatingUserId.Returns(Xrm.AdminUser.Id);
        context.OrganizationId.Returns(Guid.NewGuid());
        context.CorrelationId.Returns(Guid.NewGuid());
        context.InputParameters.Returns(new ParameterCollection { ["Target"] = target });
        var images = new EntityImageCollection();
        if (preImage != null)
        {
            images["PreImage"] = preImage;
        }

        context.PreEntityImages.Returns(images);
        var factory = Substitute.For<IOrganizationServiceFactory>();
        factory.CreateOrganizationService(Arg.Any<Guid?>()).Returns(call => call.Arg<Guid?>() is { } user ? Xrm.CreateOrganizationService(user) : Xrm.GetAdminService());
        var provider = Substitute.For<IServiceProvider>();
        provider.GetService(typeof(IPluginExecutionContext)).Returns(context);
        provider.GetService(typeof(IOrganizationServiceFactory)).Returns(factory);
        provider.GetService(typeof(ITracingService)).Returns(Substitute.For<ITracingService>());
        new RulePlugin().Execute(provider);
    }
}
