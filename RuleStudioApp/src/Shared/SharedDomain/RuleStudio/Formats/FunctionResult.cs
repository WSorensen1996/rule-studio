using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>Uniform custom function result contract.</summary>
[DataContract]
public sealed class FunctionResult
{
    [DataMember(Name = "success", EmitDefaultValue = false)]
    public bool Success { get; set; }

    [DataMember(Name = "value", EmitDefaultValue = false)]
    public object? Value { get; set; }

    [DataMember(Name = "message", EmitDefaultValue = false)]
    public string? Message { get; set; }
}
