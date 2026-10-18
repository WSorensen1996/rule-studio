using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>The rule version a trace belongs to (added 2026-10-16 with rs_SimulateRule, pending review).</summary>
[DataContract]
public sealed class TraceRule
{
    [DataMember(Name = "ruleId", EmitDefaultValue = false)]
    public Guid RuleId { get; set; }

    [DataMember(Name = "ruleKey", EmitDefaultValue = false)]
    public string RuleKey { get; set; } = string.Empty;

    /// <summary>Display only; captured at run time like record.name.</summary>
    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string? Name { get; set; }

    [DataMember(Name = "ruleVersionId", EmitDefaultValue = false)]
    public Guid RuleVersionId { get; set; }

    [DataMember(Name = "versionNumber", EmitDefaultValue = false)]
    public int VersionNumber { get; set; }

    [DataMember(Name = "priority")]
    public int Priority { get; set; }

    /// <summary>callingUser or system: the identity the rule read, wrote and called functions with.</summary>
    [DataMember(Name = "runAs", EmitDefaultValue = false)]
    public string? RunAs { get; set; }

    /// <summary>Test runs only: the draft was evaluated instead of the published version.</summary>
    [DataMember(Name = "draft", EmitDefaultValue = false)]
    public bool Draft { get; set; }
}
