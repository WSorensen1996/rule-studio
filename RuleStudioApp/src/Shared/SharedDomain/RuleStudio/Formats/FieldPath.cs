using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>Route from the triggering record to a column; also the format of rs_fieldpath and rs_valuepath.</summary>
[DataContract]
public sealed class FieldPath
{
    /// <summary>Logical name of the start table.</summary>
    [DataMember(Name = "table", EmitDefaultValue = false)]
    public string Table { get; set; } = string.Empty;

    /// <summary>Empty for a column on the record itself.</summary>
    [DataMember(Name = "hops", EmitDefaultValue = false)]
    public IList<FieldPathHop> Hops { get; } = new List<FieldPathHop>();

    /// <summary>Logical name of the column read at the end of the path.</summary>
    [DataMember(Name = "column", EmitDefaultValue = false)]
    public string Column { get; set; } = string.Empty;
}
