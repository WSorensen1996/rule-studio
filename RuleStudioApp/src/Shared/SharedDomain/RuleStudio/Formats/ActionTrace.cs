using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>A performed action.</summary>
[DataContract]
public sealed class ActionTrace
{
    [DataMember(Name = "id", EmitDefaultValue = false)]
    public Guid Id { get; set; }

    /// <summary>setField, blockSave, callFunction or logMessage.</summary>
    [DataMember(Name = "type", EmitDefaultValue = false)]
    public string Type { get; set; } = string.Empty;

    /// <summary>done, skipped or failed.</summary>
    [DataMember(Name = "status", EmitDefaultValue = false)]
    public string Status { get; set; } = string.Empty;

    [DataMember(Name = "field", EmitDefaultValue = false)]
    public string? Field { get; set; }

    [DataMember(Name = "before", EmitDefaultValue = false)]
    public object? Before { get; set; }

    [DataMember(Name = "after", EmitDefaultValue = false)]
    public object? After { get; set; }

    [DataMember(Name = "message", EmitDefaultValue = false)]
    public string? Message { get; set; }

    /// <summary>info, warning or error.</summary>
    [DataMember(Name = "messageLevel", EmitDefaultValue = false)]
    public string? MessageLevel { get; set; }

    [DataMember(Name = "lookup", EmitDefaultValue = false)]
    public LookupTrace? Lookup { get; set; }

    [DataMember(Name = "function", EmitDefaultValue = false)]
    public FunctionTrace? Function { get; set; }
}
