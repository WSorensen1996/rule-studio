using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_condition_rs_aggregate
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("None", 1033)]
    None = 126250000,

    [EnumMember]
    [OptionSetMetadata("Any", 1033)]
    Any = 126250001,

    [EnumMember]
    [OptionSetMetadata("All", 1033)]
    All = 126250002,

    [EnumMember]
    [OptionSetMetadata("None-of", 1033)]
    Noneof = 126250003,

    [EnumMember]
    [OptionSetMetadata("Count", 1033)]
    Count = 126250004,

    [EnumMember]
    [OptionSetMetadata("Sum", 1033)]
    Sum = 126250005,

    [EnumMember]
    [OptionSetMetadata("Min", 1033)]
    Min = 126250006,

    [EnumMember]
    [OptionSetMetadata("Max", 1033)]
    Max = 126250007,

    [EnumMember]
    [OptionSetMetadata("Average", 1033)]
    Average = 126250008,
}
