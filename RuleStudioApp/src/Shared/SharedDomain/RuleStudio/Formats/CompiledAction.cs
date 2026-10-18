using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>One then or else action of a compiled rule.</summary>
[DataContract]
public sealed class CompiledAction
{
    [DataMember(Name = "id", EmitDefaultValue = false, Order = 0)]
    public Guid Id { get; set; }

    /// <summary>setField, blockSave, callFunction or logMessage.</summary>
    [DataMember(Name = "type", EmitDefaultValue = false, Order = 1)]
    public string Type { get; set; } = string.Empty;

    /// <summary>setField only: column on the triggering record.</summary>
    [DataMember(Name = "targetField", EmitDefaultValue = false, Order = 2)]
    public string? TargetField { get; set; }

    /// <summary>setField: the new value. callFunction: the function.</summary>
    [DataMember(Name = "value", EmitDefaultValue = false, Order = 3)]
    public ValueSource? Value { get; set; }

    [DataMember(Name = "inputs", EmitDefaultValue = false, Order = 4)]
    public IList<CompiledInput> Inputs { get; } = new List<CompiledInput>();

    /// <summary>blockSave and logMessage.</summary>
    [DataMember(Name = "message", EmitDefaultValue = false, Order = 5)]
    public string? Message { get; set; }

    /// <summary>logMessage: info, warning or error.</summary>
    [DataMember(Name = "messageLevel", EmitDefaultValue = false, Order = 6)]
    public string? MessageLevel { get; set; }
}
