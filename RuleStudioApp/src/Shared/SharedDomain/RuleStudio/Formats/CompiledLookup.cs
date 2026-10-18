using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>Copy of a lookup definition taken at publish.</summary>
[DataContract]
public sealed class CompiledLookup
{
    [DataMember(Name = "id", EmitDefaultValue = false, Order = 0)]
    public Guid Id { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false, Order = 1)]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "table", EmitDefaultValue = false, Order = 2)]
    public string Table { get; set; } = string.Empty;

    [DataMember(Name = "returnColumn", EmitDefaultValue = false, Order = 3)]
    public string ReturnColumn { get; set; } = string.Empty;

    [DataMember(Name = "sortColumn", EmitDefaultValue = false, Order = 4)]
    public string? SortColumn { get; set; }

    /// <summary>block, useDefault or skip.</summary>
    [DataMember(Name = "noMatch", EmitDefaultValue = false, Order = 5)]
    public string NoMatch { get; set; } = string.Empty;

    /// <summary>Fixed value used when noMatch is useDefault.</summary>
    [DataMember(Name = "defaultValue", EmitDefaultValue = false, Order = 6)]
    public object? DefaultValue { get; set; }

    /// <summary>mostSpecific, firstBySort or error.</summary>
    [DataMember(Name = "multiMatch", EmitDefaultValue = false, Order = 7)]
    public string MultiMatch { get; set; } = string.Empty;

    [DataMember(Name = "matchCriteria", EmitDefaultValue = false, Order = 8)]
    public MatchCriteria? MatchCriteria { get; set; }
}
