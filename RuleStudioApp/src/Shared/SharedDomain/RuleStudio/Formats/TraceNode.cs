using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>Condition-tree node: a group (operator, children) or a condition (path, operator, expected, actual).</summary>
[DataContract]
public sealed class TraceNode
{
    /// <summary>group or condition.</summary>
    [DataMember(Name = "type", EmitDefaultValue = false)]
    public string Type { get; set; } = string.Empty;

    [DataMember(Name = "id", EmitDefaultValue = false)]
    public Guid Id { get; set; }

    /// <summary>Group: and or or. Condition: equals, notEquals, greaterThan and so on.</summary>
    [DataMember(Name = "operator", EmitDefaultValue = false)]
    public string Operator { get; set; } = string.Empty;

    [DataMember(Name = "path", EmitDefaultValue = false)]
    public FieldPath? Path { get; set; }

    /// <summary>none, any, all, noneOf, count, sum, min, max or average.</summary>
    [DataMember(Name = "aggregate", EmitDefaultValue = false)]
    public string? Aggregate { get; set; }

    [DataMember(Name = "expected", EmitDefaultValue = false)]
    public object? Expected { get; set; }

    [DataMember(Name = "actual", EmitDefaultValue = false)]
    public object? Actual { get; set; }

    /// <summary>Any, All and None-of: the number of child records checked; actual is how many matched.</summary>
    [DataMember(Name = "total", EmitDefaultValue = false)]
    public int? Total { get; set; }

    /// <summary>Null when not evaluated.</summary>
    [DataMember(Name = "result", EmitDefaultValue = false)]
    public bool? Result { get; set; }

    [DataMember(Name = "notEvaluated", EmitDefaultValue = false)]
    public bool? NotEvaluated { get; set; }

    [DataMember(Name = "lookup", EmitDefaultValue = false)]
    public LookupTrace? Lookup { get; set; }

    [DataMember(Name = "function", EmitDefaultValue = false)]
    public FunctionTrace? Function { get; set; }

    /// <summary>Group only.</summary>
    [DataMember(Name = "children", EmitDefaultValue = false)]
    public IList<TraceNode> Children { get; } = new List<TraceNode>();
}
