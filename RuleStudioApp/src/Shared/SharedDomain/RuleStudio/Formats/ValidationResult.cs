using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>rs_release.rs_validationresult; also returned by Validate-only.</summary>
[DataContract]
public sealed class ValidationResult
{
    /// <summary>Format version; <see cref="FormatVersion.Current"/> for v1 documents.</summary>
    [DataMember(Name = "version", EmitDefaultValue = false)]
    public int Version { get; set; } = FormatVersion.Current;

    /// <summary>UTC.</summary>
    [DataMember(Name = "checkedOn", EmitDefaultValue = false)]
    public DateTime CheckedOn { get; set; }

    /// <summary>passed, passedWithWarnings or failed.</summary>
    [DataMember(Name = "outcome", EmitDefaultValue = false)]
    public string Outcome { get; set; } = string.Empty;

    [DataMember(Name = "rules", EmitDefaultValue = false)]
    public IList<ValidatedRule> Rules { get; } = new List<ValidatedRule>();

    [DataMember(Name = "issues", EmitDefaultValue = false)]
    public IList<ValidationIssue> Issues { get; } = new List<ValidationIssue>();
}
