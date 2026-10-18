using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>A mapped function input.</summary>
[DataContract]
public sealed class CompiledInput
{
    /// <summary>Custom API request parameter unique name.</summary>
    [DataMember(Name = "name", EmitDefaultValue = false, Order = 0)]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "value", EmitDefaultValue = false, Order = 1)]
    public ValueSource? Value { get; set; }
}
