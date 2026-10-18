using DataverseLogic.RuleStudio.Steps;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using RuleStudio.SharedContext;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Release;

/// <summary>
/// rs_ValidateRelease and rs_PublishRelease (docs/custom-apis.md). Runs as the calling user, so
/// the caller's role decides whether they may publish. Publishing runs in the Custom API's
/// transaction: any exception rolls the whole release back. After publishing, the engine's plug-in
/// steps are reconciled in the same transaction (docs/engine.md).
/// </summary>
public class ReleaseService(IUserDataverseAccessObjectService dao, IPluginExecutionContext context, StepReconciler steps)
{
    private readonly IUserDataverseAccessObjectService dao = dao;
    private readonly IPluginExecutionContext context = context;
    private readonly StepReconciler steps = steps;

    // Checks the drafts of the rules in ruleIdsJson; returns rs_validationresult.
    public string Validate(string ruleIdsJson)
    {
        var input = new DraftLoader(dao).Load(ParseIds(ruleIdsJson));
        return Json.Write(new ReleaseValidator(dao).Validate(input, Now()));
    }

    // Validates and, unless that fails, publishes. Returns the validation result and the release id (empty when not published).
    public (string ValidationResult, Guid ReleaseId) Publish(string ruleIdsJson, string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidPluginExecutionException("A release needs a name.");

        var now = Now();
        var input = new DraftLoader(dao).Load(ParseIds(ruleIdsJson));
        var result = new ReleaseValidator(dao).Validate(input, now);
        var text = Json.Write(result);
        if (result.Outcome == "failed")
            return (text, Guid.Empty);

        var user = new EntityReference(SystemUser.EntityLogicalName, context.InitiatingUserId);
        var releaseId = dao.Create(new rs_Release
        {
            rs_Name = name.Trim(),
            rs_Description = string.IsNullOrWhiteSpace(description) ? null : description,
            rs_Status = rs_release_rs_status.Live,
            rs_PublishedOn = now,
            rs_PublishedBy = user,
            rs_ValidationResult = text,
        });

        foreach (var draft in input.Drafts)
        {
            var cost = result.Rules.First(r => r.RuleVersionId == draft.Version.Id).CostLevel;
            PublishDraft(draft, input, new Published(releaseId, now, user, cost));
        }

        text = AddStepWarnings(result, text, releaseId, steps.Reconcile());
        return (text, releaseId);
    }

    // A JSON array of rule ids; throws when it is not one.
    internal static Guid[] ParseIds(string json)
    {
        string[]? ids;
        try
        {
            ids = JsonConvert.DeserializeObject<string[]>(json ?? string.Empty);
        }
        catch (JsonException)
        {
            ids = null;
        }

        var guids = (ids ?? []).Select(id => Guid.TryParse(id, out var guid) ? guid : Guid.Empty).Distinct().ToArray();
        if (guids.Length == 0 || guids.Contains(Guid.Empty))
            throw new InvalidPluginExecutionException("RuleIds must be a JSON array of one or more rule ids.");

        return guids;
    }

    // UTC, to the second, as the JSON formats store times.
    internal static DateTime Now()
    {
        var now = DateTime.UtcNow;
        return new DateTime(now.Ticks - (now.Ticks % TimeSpan.TicksPerSecond), DateTimeKind.Utc);
    }

    // Reconcile warnings (engine not deployed, unsupported table) join the release's validation
    // result, so the publish dialog and the release show them; the publish itself still succeeds.
    private string AddStepWarnings(ValidationResult result, string text, Guid releaseId, ReconcileResult reconciled)
    {
        if (reconciled.Warnings.Count == 0)
            return text;

        foreach (var warning in reconciled.Warnings)
        {
            result.Issues.Add(warning);
        }

        if (string.Equals(result.Outcome, "passed", StringComparison.Ordinal))
        {
            result.Outcome = "passedWithWarnings";
        }

        var updated = Json.Write(result);
        dao.Update(new rs_Release(releaseId) { rs_ValidationResult = updated });
        return updated;
    }

    private void PublishDraft(RuleDraft draft, ReleaseInput input, Published published)
    {
        var snapshot = Json.Write(SnapshotCompiler.Compile(draft, input));
        dao.Update(new rs_RuleVersion(draft.Version.Id)
        {
            rs_Status = rs_ruleversion_rs_status.Published,
            rs_CompiledSnapshot = snapshot,
            rs_SnapshotHash = Json.Hash(snapshot),
            rs_CostLevel = published.CostLevel switch
            {
                "high" => rs_ruleversion_rs_costlevel.High,
                "medium" => rs_ruleversion_rs_costlevel.Medium,
                _ => rs_ruleversion_rs_costlevel.Low,
            },
            rs_ReleaseId = new EntityReference(rs_Release.EntityLogicalName, published.ReleaseId),
            rs_PublishedOn = published.On,
            rs_PublishedBy = published.By,
        });

        if (draft.Rule.rs_PublishedVersionId is { } previous)
        {
            dao.Update(new rs_RuleVersion(previous.Id) { rs_Status = rs_ruleversion_rs_status.Superseded });
        }

        dao.Update(new rs_Rule(draft.Rule.Id)
        {
            rs_PublishedVersionId = new EntityReference(rs_RuleVersion.EntityLogicalName, draft.Version.Id),
            rs_DraftVersionId = null,
        });
    }

    private sealed record Published(Guid ReleaseId, DateTime On, EntityReference By, string? CostLevel);
}
