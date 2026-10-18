using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>rs_SimulateRule with a LookupDefinitionId: one lookup evaluated against a record.</summary>
[DataContract]
public sealed class LookupTestResult
{
    [DataMember(Name = "lookup", EmitDefaultValue = false)]
    public LookupTrace? Lookup { get; set; }

    /// <summary>Set when the lookup failed, for example by its Block or Error policy.</summary>
    [DataMember(Name = "error", EmitDefaultValue = false)]
    public TraceError? Error { get; set; }
}
