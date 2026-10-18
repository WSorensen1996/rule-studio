using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>rs_executionlog.rs_trace: one rule version evaluated against one record.</summary>
[DataContract]
public sealed class ExecutionTrace
{
    /// <summary>Format version; <see cref="FormatVersion.Current"/> for v1 documents.</summary>
    [DataMember(Name = "version", EmitDefaultValue = false)]
    public int Version { get; set; } = FormatVersion.Current;

    /// <summary>summary or fullTrace.</summary>
    [DataMember(Name = "level", EmitDefaultValue = false)]
    public string Level { get; set; } = string.Empty;

    /// <summary>Position in the chain for this correlation (1-based).</summary>
    [DataMember(Name = "sequence", EmitDefaultValue = false)]
    public int Sequence { get; set; }

    /// <summary>The rule version evaluated (pending review).</summary>
    [DataMember(Name = "rule", EmitDefaultValue = false)]
    public TraceRule? Rule { get; set; }

    [DataMember(Name = "record", EmitDefaultValue = false)]
    public RecordReference Record { get; set; } = new();

    /// <summary>UTC.</summary>
    [DataMember(Name = "startedOn", EmitDefaultValue = false)]
    public DateTime StartedOn { get; set; }

    [DataMember(Name = "durationMs", EmitDefaultValue = false)]
    public int DurationMs { get; set; }

    /// <summary>matched, notMatched, skipped, failed or blocked.</summary>
    [DataMember(Name = "outcome", EmitDefaultValue = false)]
    public string Outcome { get; set; } = string.Empty;

    /// <summary>ruleDisabled, columnsNotChanged or stoppedByEarlierRule.</summary>
    [DataMember(Name = "skipReason", EmitDefaultValue = false)]
    public string? SkipReason { get; set; }

    /// <summary>Present only for test runs.</summary>
    [DataMember(Name = "test", EmitDefaultValue = false)]
    public TraceTest? Test { get; set; }

    /// <summary>Root condition group.</summary>
    [DataMember(Name = "conditions", EmitDefaultValue = false)]
    public TraceNode? Conditions { get; set; }

    /// <summary>then, else or none.</summary>
    [DataMember(Name = "branch", EmitDefaultValue = false)]
    public string? Branch { get; set; }

    [DataMember(Name = "actions", EmitDefaultValue = false)]
    public IList<ActionTrace> Actions { get; } = new List<ActionTrace>();

    [DataMember(Name = "error", EmitDefaultValue = false)]
    public TraceError? Error { get; set; }

    [DataMember(Name = "truncated", EmitDefaultValue = false)]
    public bool? Truncated { get; set; }
}
