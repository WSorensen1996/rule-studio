using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>Row an issue refers to.</summary>
[DataContract]
public sealed class IssueLocation
{
    /// <summary>trigger, conditionGroup, condition, action, inputMapping or release.</summary>
    [DataMember(Name = "kind", EmitDefaultValue = false)]
    public string Kind { get; set; } = string.Empty;

    [DataMember(Name = "id", EmitDefaultValue = false)]
    public Guid Id { get; set; }
}
