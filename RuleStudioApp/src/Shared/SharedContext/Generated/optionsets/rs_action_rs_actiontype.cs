using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_action_rs_actiontype
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Set field", 1033)]
    Setfield = 126250000,

    [EnumMember]
    [OptionSetMetadata("Block save", 1033)]
    Blocksave = 126250001,

    [EnumMember]
    [OptionSetMetadata("Call function", 1033)]
    Callfunction = 126250002,

    [EnumMember]
    [OptionSetMetadata("Log message", 1033)]
    Logmessage = 126250003,
}
