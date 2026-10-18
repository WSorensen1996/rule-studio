using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>Copy of a custom function registration taken at publish.</summary>
[DataContract]
public sealed class CompiledFunction
{
    [DataMember(Name = "id", EmitDefaultValue = false, Order = 0)]
    public Guid Id { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false, Order = 1)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Custom API unique name.</summary>
    [DataMember(Name = "apiName", EmitDefaultValue = false, Order = 2)]
    public string ApiName { get; set; } = string.Empty;

    /// <summary>text, number, money, boolean, date or lookup.</summary>
    [DataMember(Name = "outputType", EmitDefaultValue = false, Order = 3)]
    public string? OutputType { get; set; }

    [DataMember(Name = "hasSideEffects", Order = 4)]
    public bool HasSideEffects { get; set; }

    [DataMember(Name = "safeToRetry", Order = 5)]
    public bool SafeToRetry { get; set; }

    [DataMember(Name = "timeoutMs", EmitDefaultValue = false, Order = 6)]
    public int? TimeoutMs { get; set; }

    [DataMember(Name = "inputSchema", EmitDefaultValue = false, Order = 7)]
    public InputSchema? InputSchema { get; set; }
}
