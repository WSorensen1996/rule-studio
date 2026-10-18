using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>A lookup definition of the bundle, matched by name.</summary>
[DataContract]
public sealed class ImportLookupResult
{
    [DataMember(Name = "name", EmitDefaultValue = false, Order = 0)]
    public string Name { get; set; } = string.Empty;

    /// <summary>create, update or unchanged.</summary>
    [DataMember(Name = "status", EmitDefaultValue = false, Order = 1)]
    public string Status { get; set; } = string.Empty;
}
