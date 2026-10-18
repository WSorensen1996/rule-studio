using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>A rule version that was checked.</summary>
[DataContract]
public sealed class ValidatedRule
{
    [DataMember(Name = "ruleId", EmitDefaultValue = false)]
    public Guid RuleId { get; set; }

    [DataMember(Name = "ruleKey", EmitDefaultValue = false)]
    public string RuleKey { get; set; } = string.Empty;

    [DataMember(Name = "ruleVersionId", EmitDefaultValue = false)]
    public Guid RuleVersionId { get; set; }

    [DataMember(Name = "versionNumber", EmitDefaultValue = false)]
    public int VersionNumber { get; set; }

    /// <summary>low, medium or high; written to rs_costlevel on publish.</summary>
    [DataMember(Name = "costLevel", EmitDefaultValue = false)]
    public string? CostLevel { get; set; }
}
