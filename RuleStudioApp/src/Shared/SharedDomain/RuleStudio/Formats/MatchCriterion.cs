using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>One condition a lookup row must meet.</summary>
[DataContract]
public sealed class MatchCriterion
{
    /// <summary>equals, in, range or isEmpty.</summary>
    [DataMember(Name = "operator", EmitDefaultValue = false)]
    public string Operator { get; set; } = string.Empty;

    /// <summary>Lookup-table column (equals, in, isEmpty).</summary>
    [DataMember(Name = "column", EmitDefaultValue = false)]
    public string? Column { get; set; }

    /// <summary>Range lower bound column (inclusive).</summary>
    [DataMember(Name = "minColumn", EmitDefaultValue = false)]
    public string? MinColumn { get; set; }

    /// <summary>Range upper bound column (exclusive).</summary>
    [DataMember(Name = "maxColumn", EmitDefaultValue = false)]
    public string? MaxColumn { get; set; }

    /// <summary>Value compared (equals, range); fixed, fieldPath or expression only.</summary>
    [DataMember(Name = "value", EmitDefaultValue = false)]
    public ValueSource? Value { get; set; }

    /// <summary>Allowed values (in).</summary>
    [DataMember(Name = "values", EmitDefaultValue = false)]
    public IList<ValueSource> Values { get; } = new List<ValueSource>();

    /// <summary>An empty row column matches any value (wildcard).</summary>
    [DataMember(Name = "emptyMatchesAll", EmitDefaultValue = false)]
    public bool? EmptyMatchesAll { get; set; }
}
