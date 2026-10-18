using DataverseLogic.RuleStudio.Release;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json.Linq;
using RuleStudio.SharedContext;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Promotion;

/// <summary>Writes one incoming rule as a new draft version (source Imported), creating the rule when it is new.</summary>
internal sealed class DraftImporter(IDataverseAccessObject dao)
{
    private readonly IDataverseAccessObject dao = dao;

    // Returns the rule id. An existing draft is discarded and kept in history.
    internal Guid Write(rs_Rule? rule, string key, JObject doc, CompiledRule compiled, string hash)
    {
        var name = doc["name"]?.ToString();
        var description = doc["description"]?.Type == JTokenType.String ? doc["description"]!.ToString() : null;
        Guid ruleId;
        if (rule == null)
        {
            ruleId = dao.Create(new rs_Rule
            {
                rs_Name = string.IsNullOrWhiteSpace(name) ? key : name,
                rs_RuleKey = key,
                rs_Description = description,
                rs_Enabled = doc["enabled"]?.Type != JTokenType.Boolean || (bool)doc["enabled"]!,
            });
        }
        else
        {
            ruleId = rule.Id;
            if (rule.rs_DraftVersionId != null)
            {
                dao.Update(new rs_RuleVersion(rule.rs_DraftVersionId.Id) { rs_Status = rs_ruleversion_rs_status.Discarded });
            }

            dao.Update(new rs_Rule(ruleId) { rs_Name = string.IsNullOrWhiteSpace(name) ? rule.rs_Name : name, rs_Description = description });
        }

        var number = dao.RetrieveMultipleByIds<rs_RuleVersion>([ruleId], "rs_ruleid", v => v.rs_VersionNumber)
            .Select(v => v.rs_VersionNumber ?? 0).DefaultIfEmpty(0).Max() + 1;
        var trigger = compiled.Trigger;
        var runtime = compiled.Runtime;
        var versionId = dao.Create(new rs_RuleVersion
        {
            rs_Name = $"{key} v{number}",
            rs_RuleId = new EntityReference(rs_Rule.EntityLogicalName, ruleId),
            rs_VersionNumber = number,
            rs_Status = rs_ruleversion_rs_status.Draft,
            rs_Source = rs_ruleversion_rs_source.Imported,
            rs_BaseHash = hash,
            rs_ChangeNote = "Imported by rs_ImportRules.",
            rs_TargetTable = trigger.Table,
            rs_Event = ChoiceValues.Parse<rs_event>(trigger.Event, ChoiceNames.Event),
            rs_Stage = ChoiceValues.Parse<rs_stage>(trigger.Stage, ChoiceNames.Stage),
            rs_FilteredColumns = trigger.FilteredColumns.Count == 0 ? null : string.Join(",", trigger.FilteredColumns),
            rs_Priority = runtime.Priority,
            rs_StopProcessing = runtime.StopProcessing,
            rs_FailurePolicy = ChoiceValues.Parse<rs_ruleversion_rs_failurepolicy>(runtime.FailurePolicy, ChoiceNames.FailurePolicy),
            rs_RunAs = ChoiceValues.Parse<rs_ruleversion_rs_runas>(runtime.RunAs, ChoiceNames.RunAs),
            rs_RetryCount = runtime.RetryCount,
            rs_RetryDelayMs = runtime.RetryDelayMs,
            rs_LogLevel = ChoiceValues.Parse<rs_loglevel>(runtime.LogLevel, ChoiceNames.LogLevel),
        });
        new RowWriter(dao).Write(compiled, versionId);
        dao.Update(new rs_Rule(ruleId) { rs_DraftVersionId = new EntityReference(rs_RuleVersion.EntityLogicalName, versionId) });
        return ruleId;
    }
}
