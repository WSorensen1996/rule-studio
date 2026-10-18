using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_ruleversion_rs_runas
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Calling user", 1033)]
    Callinguser = 126250000,

    [EnumMember]
    [OptionSetMetadata("System", 1033)]
    System = 126250001,
}
