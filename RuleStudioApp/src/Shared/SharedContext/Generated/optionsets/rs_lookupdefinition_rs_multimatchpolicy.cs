using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_lookupdefinition_rs_multimatchpolicy
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Most specific", 1033)]
    Mostspecific = 126250000,

    [EnumMember]
    [OptionSetMetadata("First by sort", 1033)]
    Firstbysort = 126250001,

    [EnumMember]
    [OptionSetMetadata("Error", 1033)]
    Error = 126250002,
}
