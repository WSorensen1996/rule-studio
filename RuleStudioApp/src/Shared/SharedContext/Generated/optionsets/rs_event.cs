using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_event
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Create", 1033)]
    Create = 126250000,

    [EnumMember]
    [OptionSetMetadata("Update", 1033)]
    Update = 126250001,

    [EnumMember]
    [OptionSetMetadata("Delete", 1033)]
    Delete = 126250002,
}
