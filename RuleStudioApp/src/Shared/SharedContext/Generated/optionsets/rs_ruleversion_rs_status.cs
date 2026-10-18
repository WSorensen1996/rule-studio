using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_ruleversion_rs_status
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Draft", 1033)]
    Draft = 126250000,

    [EnumMember]
    [OptionSetMetadata("Published", 1033)]
    Published = 126250001,

    [EnumMember]
    [OptionSetMetadata("Superseded", 1033)]
    Superseded = 126250002,

    [EnumMember]
    [OptionSetMetadata("Discarded", 1033)]
    Discarded = 126250003,
}
