using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>A custom function call.</summary>
[DataContract]
public sealed class FunctionTrace
{
    [DataMember(Name = "customFunctionId", EmitDefaultValue = false)]
    public Guid CustomFunctionId { get; set; }

    [DataMember(Name = "customApiName", EmitDefaultValue = false)]
    public string CustomApiName { get; set; } = string.Empty;

    /// <summary>Parameter name to value.</summary>
    [DataMember(Name = "inputs", EmitDefaultValue = false)]
    public IDictionary<string, object?> Inputs { get; } = new Dictionary<string, object?>(StringComparer.Ordinal);

    /// <summary>Side-effect function not really called (test run).</summary>
    [DataMember(Name = "mocked", EmitDefaultValue = false)]
    public bool Mocked { get; set; }

    [DataMember(Name = "result", EmitDefaultValue = false)]
    public FunctionResult Result { get; set; } = new();

    [DataMember(Name = "durationMs", EmitDefaultValue = false)]
    public int DurationMs { get; set; }

    /// <summary>The call took longer than the function's timeout; its result was discarded.</summary>
    [DataMember(Name = "timedOut", EmitDefaultValue = false)]
    public bool TimedOut { get; set; }
}
