using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_customfunction_rs_outputtype
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Text", 1033)]
    Text = 126250000,

    [EnumMember]
    [OptionSetMetadata("Number", 1033)]
    Number = 126250001,

    [EnumMember]
    [OptionSetMetadata("Decimal", 1033)]
    @Decimal = 126250002,

    [EnumMember]
    [OptionSetMetadata("Money", 1033)]
    Money = 126250003,

    [EnumMember]
    [OptionSetMetadata("Boolean", 1033)]
    Boolean = 126250004,

    [EnumMember]
    [OptionSetMetadata("Date", 1033)]
    Date = 126250005,

    [EnumMember]
    [OptionSetMetadata("Lookup", 1033)]
    Lookup = 126250006,
}
