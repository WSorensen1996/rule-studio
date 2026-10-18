using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_ruleversion_rs_failurepolicy
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Block", 1033)]
    Block = 126250000,

    [EnumMember]
    [OptionSetMetadata("Log and continue", 1033)]
    Logandcontinue = 126250001,
}
