using DataverseLogic.RuleStudio.Steps;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using RuleStudio.SharedContext;
using SharedContext.Dao;

namespace DataverseLogic.RuleStudio.Release;

/// <summary>
/// rs_RollbackRelease (docs/custom-apis.md). Puts every rule of a live release back on the version
/// that was live before it, as a new release. Runs as the calling user in the Custom API's
/// transaction: any exception leaves everything as it was. The snapshots being restored were
/// valid when they were published, so they are not validated again. The engine's plug-in steps are
/// reconciled at the end, in the same transaction; reconcile warnings are not returned (docs/engine.md).
/// </summary>
public class RollbackService(IUserDataverseAccessObjectService dao, IPluginExecutionContext context, StepReconciler steps)
{
    private readonly IUserDataverseAccessObjectService dao = dao;
    private readonly IPluginExecutionContext context = context;
    private readonly StepReconciler steps = steps;

    // Rolls back the release; returns the id of the new rollback release.
    public Guid Rollback(Guid releaseId)
    {
        if (releaseId == Guid.Empty)
            throw new InvalidPluginExecutionException("ReleaseId is required.");

        var target = dao.RetrieveMultipleByIds<rs_Release>([releaseId]).FirstOrDefault()
            ?? throw new InvalidPluginExecutionException("The release no longer exists.");
        if (target.rs_Status != rs_release_rs_status.Live)
            throw new InvalidPluginExecutionException($"Release \"{target.rs_Name}\" is not live, so it cannot be rolled back.");

        var versions = dao.RetrieveMultipleByIds<rs_RuleVersion>([releaseId], "rs_releaseid")
            .Where(v => v.rs_RuleId != null).ToList();
        if (versions.Count == 0)
            throw new InvalidPluginExecutionException($"Release \"{target.rs_Name}\" published no rule versions.");

        var rules = dao.RetrieveMultipleByIds<rs_Rule>(versions.Select(v => v.rs_RuleId!.Id).Distinct()).ToDictionary(r => r.Id);
        var changed = versions
            .Where(v => !rules.TryGetValue(v.rs_RuleId!.Id, out var rule) || rule.rs_PublishedVersionId?.Id != v.Id)
            .Select(v => rules.TryGetValue(v.rs_RuleId!.Id, out var rule) ? rule.rs_Name ?? rule.rs_RuleKey ?? rule.Id.ToString() : v.rs_Name ?? v.Id.ToString())
            .ToList();
        if (changed.Count > 0)
            throw new InvalidPluginExecutionException($"A later release changed {string.Join(", ", changed)}. Roll back the later release first.");

        var now = ReleaseService.Now();
        var user = new EntityReference(SystemUser.EntityLogicalName, context.InitiatingUserId);
        var newReleaseId = dao.Create(new rs_Release
        {
            rs_Name = $"Rollback of {target.rs_Name}",
            rs_Status = rs_release_rs_status.Live,
            rs_PublishedOn = now,
            rs_PublishedBy = user,
            rs_RolledBackReleaseId = new EntityReference(rs_Release.EntityLogicalName, releaseId),
        });

        foreach (var version in versions)
        {
            RollbackVersion(version);
        }

        dao.Update(new rs_Release(releaseId) { rs_Status = rs_release_rs_status.Rolledback });
        steps.Reconcile();
        return newReleaseId;
    }

    private void RollbackVersion(rs_RuleVersion version)
    {
        var ruleId = version.rs_RuleId!.Id;
        var previous = PreviousVersion(ruleId, version.rs_VersionNumber);
        if (previous != null)
        {
            dao.Update(new rs_RuleVersion(previous.Id) { rs_Status = rs_ruleversion_rs_status.Published });
        }

        dao.Update(new rs_RuleVersion(version.Id) { rs_Status = rs_ruleversion_rs_status.Superseded });
        dao.Update(new rs_Rule(ruleId)
        {
            rs_PublishedVersionId = previous == null ? null : new EntityReference(rs_RuleVersion.EntityLogicalName, previous.Id),
        });
    }

    // The newest superseded version with a lower number, skipping versions whose release was itself
    // rolled back. Every new draft gets the next number and a rule has one draft at a time, so
    // number order is publish order.
    private rs_RuleVersion? PreviousVersion(Guid ruleId, int? versionNumber)
    {
        if (versionNumber == null)
            return null;

        var query = new QueryExpression(rs_RuleVersion.EntityLogicalName) { ColumnSet = new ColumnSet("rs_ruleversionid"), TopCount = 1 };
        query.Criteria.AddCondition("rs_ruleid", ConditionOperator.Equal, ruleId);
        query.Criteria.AddCondition("rs_status", ConditionOperator.Equal, (int)rs_ruleversion_rs_status.Superseded);
        query.Criteria.AddCondition("rs_versionnumber", ConditionOperator.LessThan, versionNumber.Value);
        query.Criteria.AddCondition("rs_compiledsnapshot", ConditionOperator.NotNull);
        query.AddOrder("rs_versionnumber", OrderType.Descending);
        var release = query.AddLink(rs_Release.EntityLogicalName, "rs_releaseid", "rs_releaseid", JoinOperator.Inner);
        release.LinkCriteria.AddCondition("rs_status", ConditionOperator.NotEqual, (int)rs_release_rs_status.Rolledback);
        return dao.RetrieveList<rs_RuleVersion>(query).FirstOrDefault();
    }
}
