using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>The result of rs_ImportRules (json-formats.md §6): what the check found and, after apply, what changed.</summary>
[DataContract]
public sealed class ImportReport
{
    [DataMember(Name = "version", Order = 0)]
    public int Version { get; set; } = FormatVersion.Current;

    /// <summary>check or apply.</summary>
    [DataMember(Name = "mode", EmitDefaultValue = false, Order = 1)]
    public string Mode { get; set; } = string.Empty;

    /// <summary>clean (check passed), blocked (conflicts or errors; nothing changed) or applied.</summary>
    [DataMember(Name = "outcome", EmitDefaultValue = false, Order = 2)]
    public string Outcome { get; set; } = string.Empty;

    /// <summary>The environment the bundle was exported from.</summary>
    [DataMember(Name = "source", EmitDefaultValue = false, Order = 3)]
    public string? Source { get; set; }

    /// <summary>apply only: the release that published the imported rules.</summary>
    [DataMember(Name = "releaseId", EmitDefaultValue = false, Order = 4)]
    public Guid? ReleaseId { get; set; }

    [DataMember(Name = "rules", Order = 5)]
    public IList<ImportRuleResult> Rules { get; } = new List<ImportRuleResult>();

    [DataMember(Name = "lookups", Order = 6)]
    public IList<ImportLookupResult> Lookups { get; } = new List<ImportLookupResult>();

    [DataMember(Name = "issues", Order = 7)]
    public IList<ValidationIssue> Issues { get; } = new List<ValidationIssue>();
}
