using DataverseLogic.RuleStudio.Steps;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using RuleStudio.SharedContext;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace Tests.RuleStudio;

/// <summary>The step reconciler against a stubbed DAO: what happens when Dataverse has no step filter for a table.</summary>
public class StepReconcilerTests
{
    [Fact]
    public void TableWithoutAStepFilterIsReportedAndNotCreated()
    {
        var dao = Substitute.For<IAdminDataverseAccessObjectService>();
        var versionId = Guid.NewGuid();
        var snapshot = "{\"version\":1,\"trigger\":{\"table\":\"rs_nosteps\",\"event\":\"update\",\"stage\":\"beforeSave\"}}";
        dao.RetrieveList<Entity>(Arg.Any<QueryExpression>(), Arg.Any<string>()).Returns(call => ((QueryExpression)call[0]).EntityName switch
        {
            "plugintype" => [new Entity("plugintype", Guid.NewGuid())],
            "sdkmessage" => [new Entity("sdkmessage", Guid.NewGuid())],
            _ => [],
        });
        dao.RetrieveList<rs_Rule>(Arg.Any<QueryExpression>(), Arg.Any<string>())
            .Returns([new rs_Rule(Guid.NewGuid()) { rs_Enabled = true, rs_PublishedVersionId = new EntityReference(rs_RuleVersion.EntityLogicalName, versionId) }]);
        dao.RetrieveMultipleByIds(Arg.Any<IEnumerable<Guid>>(), Arg.Any<System.Linq.Expressions.Expression<Func<rs_RuleVersion, object>>[]>())
            .Returns([new rs_RuleVersion(versionId) { rs_CompiledSnapshot = snapshot }]);

        var result = new StepReconciler(dao).Reconcile();

        result.EngineRegistered.Should().BeTrue();
        result.Created.Should().Be(0);
        result.Warnings.Should().ContainSingle(w => w.Code == ValidationCodes.StepNotSupported && w.Message.Contains("rs_nosteps", StringComparison.Ordinal));
        dao.DidNotReceive().Execute(Arg.Is<OrganizationRequest>(r => r.RequestName == "Create"), Arg.Any<string>());
    }
}
