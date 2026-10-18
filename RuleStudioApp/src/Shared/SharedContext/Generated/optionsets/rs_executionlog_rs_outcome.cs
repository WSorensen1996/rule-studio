using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_executionlog_rs_outcome
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Matched", 1033)]
    Matched = 126250000,

    [EnumMember]
    [OptionSetMetadata("Not matched", 1033)]
    Notmatched = 126250001,

    [EnumMember]
    [OptionSetMetadata("Skipped", 1033)]
    Skipped = 126250002,

    [EnumMember]
    [OptionSetMetadata("Failed", 1033)]
    Failed = 126250003,

    [EnumMember]
    [OptionSetMetadata("Blocked", 1033)]
    Blocked = 126250004,
}
