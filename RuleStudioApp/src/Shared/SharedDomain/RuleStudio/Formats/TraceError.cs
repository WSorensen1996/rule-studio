using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>Failure details.</summary>
[DataContract]
public sealed class TraceError
{
    [DataMember(Name = "code", EmitDefaultValue = false)]
    public string Code { get; set; } = string.Empty;

    [DataMember(Name = "message", EmitDefaultValue = false)]
    public string Message { get; set; } = string.Empty;

    /// <summary>1-based.</summary>
    [DataMember(Name = "attempt", EmitDefaultValue = false)]
    public int Attempt { get; set; }
}
