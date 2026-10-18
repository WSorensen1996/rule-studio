extern alias engine;

using DG.Tools.XrmMockup;
using engine::RuleStudio.Engine;
using Microsoft.Xrm.Sdk;
using RuleStudio.SharedContext;

namespace Tests.RuleStudio;

/// <summary>
/// The engine plug-in end to end in XrmMockup: published rules are loaded from Dataverse, run on a
/// simulated save with the calling user's or the system's service, and logged.
/// </summary>
public class EngineTests : RuleStudioTestBase
{
    private const int PreOperation = 20;

    public EngineTests(XrmMockupFixture fixture)
        : base(fixture)
    {
        RuleCatalog.Clear();
    }

    [Fact]
    public void CreateRuleSetsTheFieldAndWritesAFullTraceLog()
    {
        var ruleId = Rows.NewRule("TEST.R1");
        var version = Rows.Draft(ruleId, "account", rs_event.Create, rs_stage.Beforesave);
        Rows.Condition(version, RuleRows.Path("account", "numberofemployees"), rs_condition_rs_operator.Greaterthan, c => Fixed(c, 100));
        Rows.SetField(version, "description", "Enterprise");
        Rows.SetField(version, "description", "SMB", rs_action_rs_branch.Else);
        Publish(ruleId);

        var big = new Entity("account", Guid.NewGuid()) { ["name"] = "TEST – Big", ["numberofemployees"] = 500 };
        var small = new Entity("account", Guid.NewGuid()) { ["name"] = "TEST – Small", ["numberofemployees"] = 10 };
        Run("Create", big, null, Xrm.AdminUser.Id);
        Run("Create", small, null, Xrm.AdminUser.Id);

        big["description"].Should().Be("Enterprise");
        small["description"].Should().Be("SMB");
        var logs = Logs(ruleId);
        logs.Should().HaveCount(2);
        logs.Should().OnlyContain(l => l.rs_LogLevel == rs_loglevel.Fulltrace && l.rs_Trace!.Contains("\"setField\"", StringComparison.Ordinal));
    }

    [Fact]
    public void BlockSaveThrowsTheMessage()
    {
        var ruleId = Rows.NewRule("TEST.R3");
        var version = Rows.Draft(ruleId, "account", rs_event.Update, rs_stage.Beforesave);
        Rows.Condition(version, RuleRows.Path("account", "creditonhold"), rs_condition_rs_operator.Equals, c => Fixed(c, true));
        Rows.Action(version, rs_action_rs_actiontype.Blocksave, a => a.rs_Message = "Phone required for on-hold accounts");
        Publish(ruleId);

        var id = Guid.NewGuid();
        var save = () => Run("Update", new Entity("account", id) { ["creditonhold"] = true }, new Entity("account", id) { ["name"] = "TEST – On hold" }, Xrm.AdminUser.Id);
        save.Should().Throw<InvalidPluginExecutionException>().WithMessage("Phone required for on-hold accounts");
    }

    [Fact]
    public void RunAsDecidesWhetherTheParentCanBeRead()
    {
        var parent = NewAccount("Parent", a => a.CreditOnHold = true);
        var user = Guid.NewGuid();
        CreateUser(user, Xrm.RootBusinessUnit, SecurityRoles.RuleStudioAuthor);
        var path = RuleRows.Path("contact", "creditonhold", ("parent", "parentcustomerid", "account"));

        var callingUser = ParentRule("TEST.R9_USER", path, rs_ruleversion_rs_runas.Callinguser, "user");
        var system = ParentRule("TEST.R9_SYSTEM", path, rs_ruleversion_rs_runas.System, "system");
        Publish(callingUser, system);

        var contact = new Entity("contact", Guid.NewGuid()) { ["lastname"] = "TEST – Child", ["parentcustomerid"] = new EntityReference("account", parent) };
        Run("Create", contact, null, user);

        // The calling user cannot see the parent, so the path reads no value, as Dataverse filters unreadable rows.
        contact["description"].Should().Be("system", "the system rule reads the parent; the calling user's rule cannot");
        Logs(callingUser).Single().rs_Outcome.Should().Be(rs_executionlog_rs_outcome.Notmatched);
        Logs(system).Single().rs_Outcome.Should().Be(rs_executionlog_rs_outcome.Matched);
    }

    [Fact]
    public void RecursionAboveTheLimitStopsWithAReadableError()
    {
        var run = () => Run("Update", new Entity("account", Guid.NewGuid()), null, Xrm.AdminUser.Id, depth: 7);
        run.Should().Throw<InvalidPluginExecutionException>().WithMessage("*triggered each other 7 levels deep (limit 6)*");
    }

    [Fact]
    public void UnsafeFunctionIsAttemptedOnceAndSafeFailuresAreRetried()
    {
        var account = NewAccount("Stamp me");
        NewContact("Stamped", account);
        var stamp = Rows.Stamp();
        var discount = Rows.Discount();
        var unsafeRule = Rows.NewRule("TEST.R11");
        var version = Rows.Draft(unsafeRule, "account", rs_event.Update, rs_stage.Beforesave, v => (v.rs_RetryCount, v.rs_FailurePolicy) = (2, rs_ruleversion_rs_failurepolicy.Logandcontinue));
        var call = Rows.Action(version, rs_action_rs_actiontype.Callfunction, a => (a.rs_ValueSourceType, a.rs_CustomFunctionId) = (rs_valuesourcetype.Function, new EntityReference(rs_CustomFunction.EntityLogicalName, stamp)));
        Rows.Input(call, ownerIsAction: true, "AccountId", rs_valuesourcetype.Fieldpath, valuePath: RuleRows.Path("account", "accountid"));
        Rows.Input(call, ownerIsAction: true, "Text", rs_valuesourcetype.Fixed, "\"stamped\"");
        Failing(version, discount);
        var safeRule = Rows.NewRule("TEST.R4F");
        var safe = Rows.Draft(safeRule, "account", rs_event.Update, rs_stage.Beforesave, v => (v.rs_RetryCount, v.rs_FailurePolicy, v.rs_Priority) = (2, rs_ruleversion_rs_failurepolicy.Logandcontinue, 200));
        Failing(safe, discount);
        Publish(unsafeRule, safeRule);

        Run("Update", new Entity("account", account) { ["name"] = "TEST – Stamp me 2" }, new Entity("account", account) { ["name"] = "TEST – Stamp me", ["accountid"] = account }, Xrm.AdminUser.Id);

        Logs(unsafeRule).Single().rs_Attempts.Should().Be(1, "the side-effect function is not safe to retry");
        Logs(safeRule).Single().rs_Attempts.Should().Be(3);
        AdminDao.RetrieveList<Contact>(x => x.ContactSet.Where(c => c.ParentCustomerId!.Id == account)).Single().Description.Should().Be("stamped");
    }

    private void Failing(Guid version, Guid discount)
    {
        var fail = Rows.Action(version, rs_action_rs_actiontype.Callfunction, a => (a.rs_Order, a.rs_ValueSourceType, a.rs_CustomFunctionId) = (1, rs_valuesourcetype.Function, new EntityReference(rs_CustomFunction.EntityLogicalName, discount)));
        Rows.Input(fail, ownerIsAction: true, "Revenue", rs_valuesourcetype.Fixed, "-1");
        Rows.Input(fail, ownerIsAction: true, "Employees", rs_valuesourcetype.Fixed, "1");
    }

    private static void Fixed(rs_Condition condition, object value)
    {
        condition.rs_ValueSourceType = rs_valuesourcetype.Fixed;
        condition.rs_Value = Json(value);
    }

    private Guid ParentRule(string key, string path, rs_ruleversion_rs_runas runAs, string text)
    {
        var ruleId = Rows.NewRule(key);
        var version = Rows.Draft(ruleId, "contact", rs_event.Create, rs_stage.Beforesave, v =>
        {
            v.rs_RunAs = runAs;
            v.rs_FailurePolicy = rs_ruleversion_rs_failurepolicy.Logandcontinue;
            v.rs_Priority = runAs == rs_ruleversion_rs_runas.System ? 20 : 10;
        });
        Rows.Condition(version, path, rs_condition_rs_operator.Equals, c => Fixed(c, true));
        Rows.SetField(version, "description", text);
        return ruleId;
    }

    private List<rs_ExecutionLog> Logs(Guid ruleId) =>
        AdminDao.RetrieveList<rs_ExecutionLog>(x => x.rs_ExecutionLogSet.Where(l => l.rs_RuleId!.Id == ruleId));

    // One engine execution as the platform would start it before save.
    private void Run(string message, Entity target, Entity? preImage, Guid userId, int depth = 1)
    {
        var context = Substitute.For<IPluginExecutionContext>();
        context.MessageName.Returns(message);
        context.Stage.Returns(PreOperation);
        context.Depth.Returns(depth);
        context.PrimaryEntityName.Returns(target.LogicalName);
        context.PrimaryEntityId.Returns(target.Id);
        context.UserId.Returns(userId);
        context.InitiatingUserId.Returns(userId);
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
        factory.CreateOrganizationService(Arg.Any<Guid?>()).Returns(call => call.Arg<Guid?>() is { } id ? Xrm.CreateOrganizationService(id) : Xrm.GetAdminService());
        var provider = Substitute.For<IServiceProvider>();
        provider.GetService(typeof(IPluginExecutionContext)).Returns(context);
        provider.GetService(typeof(IOrganizationServiceFactory)).Returns(factory);
        provider.GetService(typeof(ITracingService)).Returns(Substitute.For<ITracingService>());
        new RulePlugin().Execute(provider);
    }
}
