using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_ruleversion_rs_source
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Authored", 1033)]
    Authored = 126250000,

    [EnumMember]
    [OptionSetMetadata("Imported", 1033)]
    Imported = 126250001,
}
