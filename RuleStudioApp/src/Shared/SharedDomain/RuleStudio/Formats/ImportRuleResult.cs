using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>The three-way comparison of one rule key.</summary>
[DataContract]
public sealed class ImportRuleResult
{
    [DataMember(Name = "ruleKey", EmitDefaultValue = false, Order = 0)]
    public string RuleKey { get; set; } = string.Empty;

    [DataMember(Name = "name", EmitDefaultValue = false, Order = 1)]
    public string? Name { get; set; }

    /// <summary>create, update, unchanged, keepTarget, conflict or targetOnly.</summary>
    [DataMember(Name = "status", EmitDefaultValue = false, Order = 2)]
    public string Status { get; set; } = string.Empty;

    [DataMember(Name = "incomingHash", EmitDefaultValue = false, Order = 3)]
    public string? IncomingHash { get; set; }

    /// <summary>Portable hash of the target's published version; absent when it has none.</summary>
    [DataMember(Name = "currentHash", EmitDefaultValue = false, Order = 4)]
    public string? CurrentHash { get; set; }

    /// <summary>rs_lastdeployedhash: what the last import deployed here.</summary>
    [DataMember(Name = "baseHash", EmitDefaultValue = false, Order = 5)]
    public string? BaseHash { get; set; }

    [DataMember(Name = "message", EmitDefaultValue = false, Order = 6)]
    public string? Message { get; set; }

    /// <summary>conflict only: the incoming portable rule, for the side-by-side diff.</summary>
    [DataMember(Name = "incoming", EmitDefaultValue = false, Order = 7)]
    public object? Incoming { get; set; }

    /// <summary>conflict only: the target's portable rule.</summary>
    [DataMember(Name = "current", EmitDefaultValue = false, Order = 8)]
    public object? Current { get; set; }
}
