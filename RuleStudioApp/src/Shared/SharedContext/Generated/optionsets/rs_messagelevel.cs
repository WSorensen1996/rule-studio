using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_messagelevel
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Info", 1033)]
    Info = 126250000,

    [EnumMember]
    [OptionSetMetadata("Warning", 1033)]
    Warning = 126250001,

    [EnumMember]
    [OptionSetMetadata("Error", 1033)]
    Error = 126250002,
}
