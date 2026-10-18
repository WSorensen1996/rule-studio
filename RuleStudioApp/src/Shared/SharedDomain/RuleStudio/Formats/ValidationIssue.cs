using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>One publish check result.</summary>
[DataContract]
public sealed class ValidationIssue
{
    /// <summary>error or warning.</summary>
    [DataMember(Name = "severity", EmitDefaultValue = false)]
    public string Severity { get; set; } = string.Empty;

    /// <summary>Stable code; see <see cref="ValidationCodes"/>. The list is open.</summary>
    [DataMember(Name = "code", EmitDefaultValue = false)]
    public string Code { get; set; } = string.Empty;

    /// <summary>Complete sentence for authors.</summary>
    [DataMember(Name = "message", EmitDefaultValue = false)]
    public string Message { get; set; } = string.Empty;

    [DataMember(Name = "ruleKey", EmitDefaultValue = false)]
    public string? RuleKey { get; set; }

    [DataMember(Name = "ruleVersionId", EmitDefaultValue = false)]
    public Guid? RuleVersionId { get; set; }

    [DataMember(Name = "location", EmitDefaultValue = false)]
    public IssueLocation? Location { get; set; }
}
