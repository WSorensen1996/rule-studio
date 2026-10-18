using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>A lookup evaluation.</summary>
[DataContract]
public sealed class LookupTrace
{
    [DataMember(Name = "lookupDefinitionId", EmitDefaultValue = false)]
    public Guid LookupDefinitionId { get; set; }

    /// <summary>Absent when nothing matched.</summary>
    [DataMember(Name = "matchedRowId", EmitDefaultValue = false)]
    public Guid? MatchedRowId { get; set; }

    [DataMember(Name = "matchCount", EmitDefaultValue = false)]
    public int MatchCount { get; set; }

    [DataMember(Name = "wildcards", EmitDefaultValue = false)]
    public int Wildcards { get; set; }

    [DataMember(Name = "value", EmitDefaultValue = false)]
    public object? Value { get; set; }

    /// <summary>usedDefault, skipped, blocked or error when a policy decided the result.</summary>
    [DataMember(Name = "policy", EmitDefaultValue = false)]
    public string? Policy { get; set; }
}
