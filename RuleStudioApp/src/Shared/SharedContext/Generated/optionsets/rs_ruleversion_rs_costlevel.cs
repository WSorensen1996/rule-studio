using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_ruleversion_rs_costlevel
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Low", 1033)]
    Low = 126250000,

    [EnumMember]
    [OptionSetMetadata("Medium", 1033)]
    Medium = 126250001,

    [EnumMember]
    [OptionSetMetadata("High", 1033)]
    High = 126250002,
}
