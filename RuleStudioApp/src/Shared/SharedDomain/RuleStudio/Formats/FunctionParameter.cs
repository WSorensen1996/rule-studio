using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>A typed input of a registered custom function.</summary>
[DataContract]
public sealed class FunctionParameter
{
    /// <summary>Custom API request parameter unique name.</summary>
    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "displayName", EmitDefaultValue = false)]
    public string? DisplayName { get; set; }

    /// <summary>text, number, decimal, money, boolean, date or lookup.</summary>
    [DataMember(Name = "type", EmitDefaultValue = false)]
    public string Type { get; set; } = string.Empty;

    /// <summary>Expected table, only for lookup.</summary>
    [DataMember(Name = "table", EmitDefaultValue = false)]
    public string? Table { get; set; }

    [DataMember(Name = "required", EmitDefaultValue = false)]
    public bool Required { get; set; }

    [DataMember(Name = "description", EmitDefaultValue = false)]
    public string? Description { get; set; }
}
