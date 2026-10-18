using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>One step from a table to a related table.</summary>
[DataContract]
public sealed class FieldPathHop
{
    /// <summary>parent (N:1 lookup on the current table) or child (1:N, lookup on the other table).</summary>
    [DataMember(Name = "kind", EmitDefaultValue = false)]
    public string Kind { get; set; } = string.Empty;

    /// <summary>Parent: lookup column on the current table. Child: lookup column on the child table.</summary>
    [DataMember(Name = "attribute", EmitDefaultValue = false)]
    public string Attribute { get; set; } = string.Empty;

    /// <summary>Logical name of the table reached.</summary>
    [DataMember(Name = "table", EmitDefaultValue = false)]
    public string Table { get; set; } = string.Empty;
}
