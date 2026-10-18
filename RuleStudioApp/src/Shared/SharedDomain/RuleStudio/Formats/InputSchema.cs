using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>rs_customfunction.rs_inputschema.</summary>
[DataContract]
public sealed class InputSchema
{
    /// <summary>Format version; <see cref="FormatVersion.Current"/> for v1 documents.</summary>
    [DataMember(Name = "version", EmitDefaultValue = false)]
    public int Version { get; set; } = FormatVersion.Current;

    [DataMember(Name = "parameters", EmitDefaultValue = false)]
    public IList<FunctionParameter> Parameters { get; } = new List<FunctionParameter>();
}
