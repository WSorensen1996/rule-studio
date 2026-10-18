using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>Test-run details.</summary>
[DataContract]
public sealed class TraceTest
{
    [DataMember(Name = "mockedFunctions", EmitDefaultValue = false)]
    public IList<string> MockedFunctions { get; } = new List<string>();
}
