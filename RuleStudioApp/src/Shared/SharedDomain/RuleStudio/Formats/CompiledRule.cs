using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>rs_ruleversion.rs_compiledsnapshot: everything the engine needs to run one published version.</summary>
[DataContract]
public sealed class CompiledRule
{
    /// <summary>Format version; <see cref="FormatVersion.Current"/> for v1 documents.</summary>
    [DataMember(Name = "version", EmitDefaultValue = false, Order = 0)]
    public int Version { get; set; } = FormatVersion.Current;

    [DataMember(Name = "ruleId", EmitDefaultValue = false, Order = 1)]
    public Guid RuleId { get; set; }

    [DataMember(Name = "ruleKey", EmitDefaultValue = false, Order = 2)]
    public string RuleKey { get; set; } = string.Empty;

    [DataMember(Name = "ruleVersionId", EmitDefaultValue = false, Order = 3)]
    public Guid RuleVersionId { get; set; }

    [DataMember(Name = "versionNumber", EmitDefaultValue = false, Order = 4)]
    public int VersionNumber { get; set; }

    [DataMember(Name = "trigger", EmitDefaultValue = false, Order = 5)]
    public RuleTrigger Trigger { get; set; } = new();

    [DataMember(Name = "runtime", EmitDefaultValue = false, Order = 6)]
    public RuleRuntime Runtime { get; set; } = new();

    /// <summary>Root condition group; an empty group always matches.</summary>
    [DataMember(Name = "conditions", EmitDefaultValue = false, Order = 7)]
    public CompiledNode Conditions { get; set; } = new();

    [DataMember(Name = "then", Order = 8)]
    public IList<CompiledAction> Then { get; } = new List<CompiledAction>();

    [DataMember(Name = "else", Order = 9)]
    public IList<CompiledAction> Else { get; } = new List<CompiledAction>();

    [DataMember(Name = "lookups", EmitDefaultValue = false, Order = 10)]
    public IList<CompiledLookup> Lookups { get; } = new List<CompiledLookup>();

    [DataMember(Name = "functions", EmitDefaultValue = false, Order = 11)]
    public IList<CompiledFunction> Functions { get; } = new List<CompiledFunction>();
}
