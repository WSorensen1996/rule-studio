using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_release_rs_status
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Live", 1033)]
    Live = 126250000,

    [EnumMember]
    [OptionSetMetadata("Rolled back", 1033)]
    Rolledback = 126250001,

    [EnumMember]
    [OptionSetMetadata("Failed", 1033)]
    Failed = 126250002,
}
