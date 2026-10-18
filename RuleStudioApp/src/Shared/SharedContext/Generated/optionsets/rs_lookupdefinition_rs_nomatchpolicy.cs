using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_lookupdefinition_rs_nomatchpolicy
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Block", 1033)]
    Block = 126250000,

    [EnumMember]
    [OptionSetMetadata("Use default", 1033)]
    Usedefault = 126250001,

    [EnumMember]
    [OptionSetMetadata("Skip", 1033)]
    Skip = 126250002,
}
