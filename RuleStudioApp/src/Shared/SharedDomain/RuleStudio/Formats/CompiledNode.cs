using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>Condition-tree node: a group (operator, children) or a condition (path, aggregate, operator, value).</summary>
[DataContract]
public sealed class CompiledNode
{
    /// <summary>group or condition.</summary>
    [DataMember(Name = "type", EmitDefaultValue = false, Order = 0)]
    public string Type { get; set; } = string.Empty;

    [DataMember(Name = "id", EmitDefaultValue = false, Order = 1)]
    public Guid Id { get; set; }

    [DataMember(Name = "path", EmitDefaultValue = false, Order = 2)]
    public FieldPath? Path { get; set; }

    /// <summary>none, any, all, noneOf, count, sum, min, max or average.</summary>
    [DataMember(Name = "aggregate", EmitDefaultValue = false, Order = 3)]
    public string? Aggregate { get; set; }

    /// <summary>Group: and or or. Condition: equals, notEquals, greaterThan and so on.</summary>
    [DataMember(Name = "operator", EmitDefaultValue = false, Order = 4)]
    public string Operator { get; set; } = string.Empty;

    /// <summary>Condition only; absent for isEmpty, isNotEmpty and changed.</summary>
    [DataMember(Name = "value", EmitDefaultValue = false, Order = 5)]
    public ValueSource? Value { get; set; }

    /// <summary>Function inputs when the value is a function.</summary>
    [DataMember(Name = "inputs", EmitDefaultValue = false, Order = 6)]
    public IList<CompiledInput> Inputs { get; } = new List<CompiledInput>();

    /// <summary>Group only.</summary>
    [DataMember(Name = "children", EmitDefaultValue = false, Order = 7)]
    public IList<CompiledNode> Children { get; } = new List<CompiledNode>();
}
