using System.Runtime.Serialization;

namespace SharedDomain.RuleStudio.Formats;

/// <summary>rs_lookupdefinition.rs_matchcriteria.</summary>
[DataContract]
public sealed class MatchCriteria
{
    /// <summary>Format version; <see cref="FormatVersion.Current"/> for v1 documents.</summary>
    [DataMember(Name = "version", EmitDefaultValue = false)]
    public int Version { get; set; } = FormatVersion.Current;

    /// <summary>Table of the record the lookup is evaluated for; every field path starts here.</summary>
    [DataMember(Name = "recordTable", EmitDefaultValue = false)]
    public string RecordTable { get; set; } = string.Empty;

    /// <summary>All must hold (AND).</summary>
    [DataMember(Name = "criteria", EmitDefaultValue = false)]
    public IList<MatchCriterion> Criteria { get; } = new List<MatchCriterion>();
}
