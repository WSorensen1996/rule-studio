using DG.Tools.XrmMockup;
using Microsoft.Xrm.Sdk;
using RuleStudio.SharedContext;

namespace Tests.RuleStudio;

/// <summary>Operations in XrmMockup: enabling and disabling, log retention and the admin flag.</summary>
public class OperationsTests : RuleStudioTestBase
{
    public OperationsTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public void EnablingAndDisablingReconcilesWithoutFailing()
    {
        var rule = Rows.NewRule("TEST.ENABLED");
        var version = Rows.Draft(rule, "account", rs_event.Update, rs_stage.Beforesave);
        Rows.SetField(version, "description", "x");
        Publish(rule);

        // The reconcile plug-in runs on these updates; without the engine it only reports it.
        AdminDao.Update(new rs_Rule(rule) { rs_Enabled = false });
        AdminDao.Update(new rs_Rule(rule) { rs_Enabled = true });
        ((string)Admin.Execute(Api("rs_ReconcileSteps"))["Summary"]).Should().Contain("not deployed");
    }

    [Fact]
    public void RetentionRejectsOutOfRangeDays()
    {
        var call = () => Admin.Execute(Api("rs_ConfigureLogRetention", ("Days", 5000)));
        call.Should().Throw<Exception>().WithMessage("*between 1 and 3650*");
    }

    [Fact]
    public void AuthorCanClearTheAdminFlagAndViewerCannot()
    {
        var log = AdminDao.Create(new rs_ExecutionLog { rs_Name = "TEST – flagged", rs_AdminFlag = true });
        var viewer = Guid.NewGuid();
        CreateUser(viewer, Xrm.RootBusinessUnit, SecurityRoles.RuleStudioViewer);
        var denied = () => Xrm.CreateOrganizationService(viewer).Update(new Entity(rs_ExecutionLog.EntityLogicalName, log) { ["rs_adminflag"] = false });
        denied.Should().Throw<Exception>();

        var author = Guid.NewGuid();
        CreateUser(author, Xrm.RootBusinessUnit, SecurityRoles.RuleStudioAuthor);
        Xrm.CreateOrganizationService(author).Update(new Entity(rs_ExecutionLog.EntityLogicalName, log) { ["rs_adminflag"] = false });
        AdminDao.Retrieve<rs_ExecutionLog>(log, l => l.rs_AdminFlag).rs_AdminFlag.Should().BeFalse();
    }
}
