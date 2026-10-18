using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_action_rs_branch
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Then", 1033)]
    Then = 126250000,

    [EnumMember]
    [OptionSetMetadata("Else", 1033)]
    @Else = 126250001,
}
