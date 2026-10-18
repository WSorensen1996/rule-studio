using DataverseLogic.RuleStudio.Engine;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Release;

/// <summary>State shared by the checks of one rule draft: where issues go and what the rule costs.</summary>
internal sealed class CheckContext
{
    internal const string Error = "error";
    internal const string Warning = "warning";

    private readonly IList<ValidationIssue> issues;

    internal CheckContext(IList<ValidationIssue> issues, TableMetadata metadata, ReleaseInput input, RuleDraft draft)
    {
        this.issues = issues;
        Metadata = metadata;
        Input = input;
        Draft = draft;
    }

    internal TableMetadata Metadata { get; }

    internal ReleaseInput Input { get; }

    internal RuleDraft Draft { get; }

    internal string Table => Draft.Version.rs_TargetTable ?? string.Empty;

    internal int Hops { get; set; }

    internal int ChildQueries { get; set; }

    internal int FunctionCalls { get; set; }

    // low, medium or high: a child query or a function call weighs three hops.
    internal string CostLevel
    {
        get
        {
            var weight = Hops + (ChildQueries * 3) + (FunctionCalls * 3);
            if (weight >= 12)
                return "high";

            return weight >= 5 ? "medium" : "low";
        }
    }

    internal void Add(string severity, string code, string message, Location location)
    {
        issues.Add(new ValidationIssue
        {
            Severity = severity,
            Code = code,
            Message = message,
            RuleKey = Draft.RuleKey,
            RuleVersionId = Draft.Version.Id,
            Location = new IssueLocation { Kind = location.Kind, Id = location.Id },
        });
    }
}
