using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>How a compiled rule runs.</summary>
[DataContract]
public sealed class RuleRuntime
{
    /// <summary>Lowest runs first within a chain.</summary>
    [DataMember(Name = "priority", Order = 0)]
    public int Priority { get; set; }

    [DataMember(Name = "stopProcessing", Order = 1)]
    public bool StopProcessing { get; set; }

    /// <summary>block or logAndContinue.</summary>
    [DataMember(Name = "failurePolicy", EmitDefaultValue = false, Order = 2)]
    public string FailurePolicy { get; set; } = string.Empty;

    /// <summary>callingUser or system.</summary>
    [DataMember(Name = "runAs", EmitDefaultValue = false, Order = 3)]
    public string RunAs { get; set; } = string.Empty;

    [DataMember(Name = "retryCount", Order = 4)]
    public int RetryCount { get; set; }

    [DataMember(Name = "retryDelayMs", Order = 5)]
    public int RetryDelayMs { get; set; }

    /// <summary>off, errorsOnly, summary or fullTrace; absent means Errors only.</summary>
    [DataMember(Name = "logLevel", EmitDefaultValue = false, Order = 6)]
    public string? LogLevel { get; set; }
}
