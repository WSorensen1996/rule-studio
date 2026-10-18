using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>A Dataverse record.</summary>
[DataContract]
public sealed class RecordReference
{
    [DataMember(Name = "table", EmitDefaultValue = false)]
    public string Table { get; set; } = string.Empty;

    [DataMember(Name = "id", EmitDefaultValue = false)]
    public Guid Id { get; set; }

    /// <summary>Display only.</summary>
    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string? Name { get; set; }
}
