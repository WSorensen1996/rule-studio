using DataverseLogic.RuleStudio.Engine;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Release;

/// <summary>Runs every publish check over the loaded drafts and builds rs_validationresult (json-formats.md §3).</summary>
internal sealed class ReleaseValidator(IDataverseAccessObject dao)
{
    private readonly IDataverseAccessObject dao = dao;

    internal ValidationResult Validate(ReleaseInput input, DateTime now)
    {
        var result = new ValidationResult { CheckedOn = now };
        var metadata = new TableMetadata(dao);
        foreach (var rule in input.WithoutDraft)
        {
            result.Issues.Add(new ValidationIssue
            {
                Severity = CheckContext.Error,
                Code = ValidationCodes.NoDraft,
                Message = $"{rule.rs_Name ?? rule.rs_RuleKey} has no draft to publish.",
                RuleKey = rule.rs_RuleKey,
            });
        }

        var contexts = input.Drafts.ToDictionary(d => d.Version.Id, d => new CheckContext(result.Issues, metadata, input, d));
        foreach (var draft in input.Drafts)
        {
            var context = contexts[draft.Version.Id];
            if (RowChecks.CheckTrigger(context))
            {
                draft.Conditions.ForEach(c => RowChecks.CheckCondition(context, c));
                draft.Actions.ForEach(a => RowChecks.CheckAction(context, a));
            }

            result.Rules.Add(new ValidatedRule
            {
                RuleId = draft.Rule.Id,
                RuleKey = draft.RuleKey,
                RuleVersionId = draft.Version.Id,
                VersionNumber = draft.Version.rs_VersionNumber ?? 0,
                CostLevel = context.CostLevel,
            });
        }

        new ChainChecks(dao).Check(input, d => contexts[d.Version.Id]);
        result.Outcome = Outcome(result.Issues);
        return result;
    }

    private static string Outcome(IList<ValidationIssue> issues)
    {
        if (issues.Any(i => i.Severity == CheckContext.Error))
            return "failed";

        return issues.Count > 0 ? "passedWithWarnings" : "passed";
    }
}
