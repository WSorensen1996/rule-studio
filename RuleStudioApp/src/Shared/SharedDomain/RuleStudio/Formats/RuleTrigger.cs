using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>When a compiled rule runs.</summary>
[DataContract]
public sealed class RuleTrigger
{
    [DataMember(Name = "table", EmitDefaultValue = false, Order = 0)]
    public string Table { get; set; } = string.Empty;

    /// <summary>create, update or delete.</summary>
    [DataMember(Name = "event", EmitDefaultValue = false, Order = 1)]
    public string Event { get; set; } = string.Empty;

    /// <summary>beforeSave or afterSave.</summary>
    [DataMember(Name = "stage", EmitDefaultValue = false, Order = 2)]
    public string Stage { get; set; } = string.Empty;

    /// <summary>Update only; empty means any column.</summary>
    [DataMember(Name = "filteredColumns", EmitDefaultValue = false, Order = 3)]
    public IList<string> FilteredColumns { get; } = new List<string>();
}
