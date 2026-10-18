using System.Runtime.Serialization;

namespace RuleStudio.SharedContext;

[System.CodeDom.Compiler.GeneratedCode("DataverseProxyGenerator", "4.0.0.0")]
[DataContract]
#pragma warning disable CS8981
public enum rs_loglevel
#pragma warning restore CS8981
{
    [EnumMember]
    [OptionSetMetadata("Off", 1033)]
    Off = 126250000,

    [EnumMember]
    [OptionSetMetadata("Errors only", 1033)]
    Errorsonly = 126250001,

    [EnumMember]
    [OptionSetMetadata("Summary", 1033)]
    Summary = 126250002,

    [EnumMember]
    [OptionSetMetadata("Full trace", 1033)]
    Fulltrace = 126250003,
}
