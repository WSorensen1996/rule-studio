using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_condition_rs_operator
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Equals", 1033)]
    Equals = 126250000,

    [EnumMember]
    [OptionSetMetadata("Not equals", 1033)]
    Notequals = 126250001,

    [EnumMember]
    [OptionSetMetadata("Greater than", 1033)]
    Greaterthan = 126250002,

    [EnumMember]
    [OptionSetMetadata("Greater or equal", 1033)]
    Greaterorequal = 126250003,

    [EnumMember]
    [OptionSetMetadata("Less than", 1033)]
    Lessthan = 126250004,

    [EnumMember]
    [OptionSetMetadata("Less or equal", 1033)]
    Lessorequal = 126250005,

    [EnumMember]
    [OptionSetMetadata("Contains", 1033)]
    Contains = 126250006,

    [EnumMember]
    [OptionSetMetadata("Starts with", 1033)]
    Startswith = 126250007,

    [EnumMember]
    [OptionSetMetadata("Is empty", 1033)]
    Isempty = 126250008,

    [EnumMember]
    [OptionSetMetadata("Is not empty", 1033)]
    Isnotempty = 126250009,

    [EnumMember]
    [OptionSetMetadata("Changed", 1033)]
    Changed = 126250010,

    [EnumMember]
    [OptionSetMetadata("In list", 1033)]
    Inlist = 126250011,

    [EnumMember]
    [OptionSetMetadata("Between", 1033)]
    Between = 126250012,
}
