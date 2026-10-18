using Microsoft.Xrm.Sdk;
using RuleStudio.SharedContext;
using SharedContext.Dao;

namespace DataverseLogic.RuleStudio.Release;

/// <summary>
/// rs_RestoreVersion (docs/custom-apis.md). Copies an earlier version's authoring rows into a new
/// draft of the same rule. Runs as the calling user in the Custom API's transaction. Snapshot,
/// hash, release and publish fields are not copied: the new draft is published like any other.
/// </summary>
public class RestoreService(IUserDataverseAccessObjectService dao)
{
    private readonly IUserDataverseAccessObjectService dao = dao;

    // Restores the version as the rule's new draft; returns the new draft's id.
    public Guid Restore(Guid ruleVersionId, bool replaceDraft)
    {
        if (ruleVersionId == Guid.Empty)
            throw new InvalidPluginExecutionException("RuleVersionId is required.");

        var source = dao.RetrieveMultipleByIds<rs_RuleVersion>([ruleVersionId]).FirstOrDefault()
            ?? throw new InvalidPluginExecutionException("The rule version no longer exists.");
        if (source.rs_RuleId == null)
            throw new InvalidPluginExecutionException("The rule version does not belong to a rule.");

        var rule = dao.RetrieveMultipleByIds<rs_Rule>([source.rs_RuleId.Id]).FirstOrDefault()
            ?? throw new InvalidPluginExecutionException("The rule no longer exists.");
        if (rule.rs_DraftVersionId?.Id == source.Id)
            throw new InvalidPluginExecutionException($"Version {source.rs_VersionNumber} is already the draft of {rule.rs_Name}.");

        if (rule.rs_DraftVersionId != null)
        {
            if (!replaceDraft)
                throw new InvalidPluginExecutionException($"This rule already has a draft. Restoring version {source.rs_VersionNumber} replaces it; the old draft is kept in history as Discarded.");

            dao.Update(new rs_RuleVersion(rule.rs_DraftVersionId.Id) { rs_Status = rs_ruleversion_rs_status.Discarded });
        }

        var number = dao.RetrieveMultipleByIds<rs_RuleVersion>([rule.Id], "rs_ruleid", v => v.rs_VersionNumber)
            .Select(v => v.rs_VersionNumber ?? 0).DefaultIfEmpty(0).Max() + 1;
        var draftId = dao.Create(new rs_RuleVersion
        {
            rs_Name = $"{rule.rs_RuleKey} v{number}",
            rs_RuleId = new EntityReference(rs_Rule.EntityLogicalName, rule.Id),
            rs_VersionNumber = number,
            rs_Status = rs_ruleversion_rs_status.Draft,
            rs_Source = rs_ruleversion_rs_source.Authored,
            rs_DerivedFromVersionId = new EntityReference(rs_RuleVersion.EntityLogicalName, source.Id),
            rs_ChangeNote = $"Restored from v{source.rs_VersionNumber}",
            rs_TargetTable = source.rs_TargetTable,
            rs_Event = source.rs_Event,
            rs_Stage = source.rs_Stage,
            rs_FilteredColumns = source.rs_FilteredColumns,
            rs_Priority = source.rs_Priority,
            rs_StopProcessing = source.rs_StopProcessing,
            rs_FailurePolicy = source.rs_FailurePolicy,
            rs_RunAs = source.rs_RunAs,
            rs_RetryCount = source.rs_RetryCount,
            rs_RetryDelayMs = source.rs_RetryDelayMs,
            rs_LogLevel = source.rs_LogLevel,
        });

        new VersionCopier(dao).CopyRows(source.Id, draftId);
        dao.Update(new rs_Rule(rule.Id) { rs_DraftVersionId = new EntityReference(rs_RuleVersion.EntityLogicalName, draftId) });
        return draftId;
    }
}
