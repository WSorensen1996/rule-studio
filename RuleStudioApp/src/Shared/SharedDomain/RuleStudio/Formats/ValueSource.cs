using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>Where a value comes from; only the property matching the type is set.</summary>
[DataContract]
public sealed class ValueSource
{
    /// <summary>fixed, fieldPath, expression, lookup or function.</summary>
    [DataMember(Name = "type", EmitDefaultValue = false)]
    public string Type { get; set; } = string.Empty;

    /// <summary>Fixed value: string, number, boolean, null or a record reference object.</summary>
    [DataMember(Name = "value", EmitDefaultValue = false)]
    public object? Value { get; set; }

    [DataMember(Name = "path", EmitDefaultValue = false)]
    public FieldPath? Path { get; set; }

    [DataMember(Name = "expression", EmitDefaultValue = false)]
    public string? Expression { get; set; }

    [DataMember(Name = "lookupDefinitionId", EmitDefaultValue = false)]
    public Guid? LookupDefinitionId { get; set; }

    [DataMember(Name = "customFunctionId", EmitDefaultValue = false)]
    public Guid? CustomFunctionId { get; set; }
}
