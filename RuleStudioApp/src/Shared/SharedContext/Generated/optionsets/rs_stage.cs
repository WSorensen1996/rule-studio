using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_stage
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Before save", 1033)]
    Beforesave = 126250000,

    [EnumMember]
    [OptionSetMetadata("After save", 1033)]
    Aftersave = 126250001,
}
