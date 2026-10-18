using DG.Tools.XrmMockup;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json.Linq;
using RuleStudio.SharedContext;

namespace Tests.RuleStudio;

/// <summary>
/// The write Custom APIs do their writes as the calling user, so a Rule Studio Viewer is
/// denied and nothing changes, while a Rule Studio Author succeeds. XrmMockup only: the environment
/// has no second user.
/// </summary>
public class SecurityTests : RuleStudioTestBase
{
    public SecurityTests(XrmMockupFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public void ViewerCanReadButCannotWrite()
    {
        var viewer = User(SecurityRoles.RuleStudioViewer);
        var ruleId = Rows.NewRule("TEST.READ");
        viewer.Retrieve(rs_Rule.EntityLogicalName, ruleId, new Microsoft.Xrm.Sdk.Query.ColumnSet("rs_name")).Should().NotBeNull();
        var write = () => viewer.Update(new Entity(rs_Rule.EntityLogicalName, ruleId) { ["rs_name"] = "changed" });
        write.Should().Throw<Exception>();
    }

    [Fact]
    public void ViewerCannotPublishAndNothingChanges()
    {
        var ruleId = DraftRule("TEST.PUBLISH");
        var publish = () => User(SecurityRoles.RuleStudioViewer).Execute(Api("rs_PublishRelease", ("RuleIds", Json(new[] { ruleId })), ("Name", "TEST – viewer")));
        publish.Should().Throw<Exception>();
        Releases().Should().Be(0);
        AdminDao.Retrieve<rs_Rule>(ruleId, r => r.rs_PublishedVersionId).rs_PublishedVersionId.Should().BeNull();

        Publish(User(SecurityRoles.RuleStudioAuthor), ruleId).Should().NotBeEmpty();
    }

    [Fact]
    public void ViewerCannotRollBackOrRestore()
    {
        var ruleId = DraftRule("TEST.ROLLBACK");
        var releaseId = Publish(ruleId);
        var versionId = AdminDao.Retrieve<rs_Rule>(ruleId, r => r.rs_PublishedVersionId).rs_PublishedVersionId!.Id;
        var viewer = User(SecurityRoles.RuleStudioViewer);

        var rollback = () => viewer.Execute(Api("rs_RollbackRelease", ("ReleaseId", releaseId)));
        rollback.Should().Throw<Exception>();
        AdminDao.Retrieve<rs_Release>(releaseId, r => r.rs_Status).rs_Status.Should().Be(rs_release_rs_status.Live);

        var restore = () => viewer.Execute(Api("rs_RestoreVersion", ("RuleVersionId", versionId)));
        restore.Should().Throw<Exception>();
        AdminDao.Retrieve<rs_Rule>(ruleId, r => r.rs_DraftVersionId).rs_DraftVersionId.Should().BeNull();

        var author = User(SecurityRoles.RuleStudioAuthor);
        author.Execute(Api("rs_RestoreVersion", ("RuleVersionId", versionId)));
        author.Execute(Api("rs_RollbackRelease", ("ReleaseId", releaseId)));
        AdminDao.Retrieve<rs_Release>(releaseId, r => r.rs_Status).rs_Status.Should().Be(rs_release_rs_status.Rolledback);
    }

    [Fact]
    public void ViewerCanCheckButNotApplyAnImport()
    {
        Publish(DraftRule("TEST.IMPORT"));
        var bundle = JObject.Parse((string)Admin.Execute(Api("rs_ExportRules"))["Bundle"]);
        var rule = (JObject)bundle["rules"]![0]!;
        rule["ruleKey"] = "TEST.IMPORTED";
        var text = bundle.ToString();
        var viewer = User(SecurityRoles.RuleStudioViewer);

        var check = JObject.Parse((string)viewer.Execute(Api("rs_ImportRules", ("Bundle", text), ("Mode", "check")))["Report"]);
        check["outcome"]!.ToString().Should().Be("clean");
        var apply = () => viewer.Execute(Api("rs_ImportRules", ("Bundle", text), ("Mode", "apply")));
        apply.Should().Throw<Exception>();
        Releases().Should().Be(1);

        var applied = JObject.Parse((string)User(SecurityRoles.RuleStudioAuthor).Execute(Api("rs_ImportRules", ("Bundle", text), ("Mode", "apply")))["Report"]);
        applied["outcome"]!.ToString().Should().Be("applied");
    }

    [Fact]
    public void ViewerCanValidateTestAndExport()
    {
        var ruleId = DraftRule("TEST.VIEWER_READS");
        var viewer = User(SecurityRoles.RuleStudioViewer);

        JObject.Parse((string)viewer.Execute(Api("rs_ValidateRelease", ("RuleIds", Json(new[] { ruleId }))))["ValidationResult"])["outcome"].Should().NotBeNull();
        JArray.Parse((string)viewer.Execute(Api("rs_SimulateRule", ("RuleIds", Json(new[] { ruleId })), ("SampleValues", Json(new { name = "x" }))))["Trace"]).Should().ContainSingle();
        JObject.Parse((string)viewer.Execute(Api("rs_ExportRules"))["Bundle"])["rules"].Should().NotBeNull();
    }

    private IOrganizationService User(Guid role)
    {
        var id = Guid.NewGuid();
        CreateUser(id, Xrm.RootBusinessUnit, role);
        return Xrm.CreateOrganizationService(id);
    }

    private int Releases() => AdminDao.RetrieveList<rs_Release>(x => x.rs_ReleaseSet).Count;

    private Guid DraftRule(string key)
    {
        var ruleId = Rows.NewRule(key);
        var versionId = Rows.Draft(ruleId, "account", rs_event.Update, rs_stage.Beforesave, v => v.rs_FilteredColumns = "name");
        Rows.SetField(versionId, "description", "set");
        return ruleId;
    }
}
