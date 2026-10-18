using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_valuesourcetype
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Fixed", 1033)]
    @Fixed = 126250000,

    [EnumMember]
    [OptionSetMetadata("Field path", 1033)]
    Fieldpath = 126250001,

    [EnumMember]
    [OptionSetMetadata("Expression", 1033)]
    Expression = 126250002,

    [EnumMember]
    [OptionSetMetadata("Lookup", 1033)]
    Lookup = 126250003,

    [EnumMember]
    [OptionSetMetadata("Function", 1033)]
    Function = 126250004,
}
