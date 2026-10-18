using Microsoft.Xrm.Sdk.Query;
using RuleStudio.SharedContext;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Release;

/// <summary>
/// Warnings across rules: two rules in one chain setting the same field, and after-save
/// updates that may trigger rules again. The chain is this release's drafts plus the live versions
/// of every other rule on the same tables.
/// </summary>
internal sealed class ChainChecks(IDataverseAccessObject dao)
{
    private readonly IDataverseAccessObject dao = dao;

    internal void Check(ReleaseInput input, Func<RuleDraft, CheckContext> contextOf)
    {
        var members = Members(input);
        foreach (var draft in input.Drafts)
        {
            var me = members.Find(m => m.VersionId == draft.Version.Id)!;
            var context = contextOf(draft);
            foreach (var action in draft.Actions.Where(a => a.rs_ActionType == rs_action_rs_actiontype.Setfield && a.rs_TargetField != null))
            {
                var at = new Location("action", action.Id, "Set field");
                var field = action.rs_TargetField!.ToLowerInvariant();
                foreach (var other in members.Where(m => m.VersionId != me.VersionId && m.Chain == me.Chain && m.SetFields.Contains(field)))
                {
                    context.Add(CheckContext.Warning, ValidationCodes.FieldSetConflict, ConflictText(field, me, other), at);
                }

                if (draft.Version.rs_Stage == rs_stage.Aftersave)
                {
                    WarnRetrigger(context, members, me, field, at);
                }
            }
        }
    }

    // When the earlier rule stops processing, the later one only applies when the earlier does not match.
    private static string ConflictText(string field, Member me, Member other)
    {
        var (earlier, later) = me.Priority <= other.Priority ? (me, other) : (other, me);
        if (me.Priority != other.Priority && earlier.StopProcessing)
            return $"Both rules set {field}. {later.Label} only overrides {earlier.Label} when {earlier.Label} doesn't match (it stops processing).";

        return $"{field} is also set by {other.Label}, {Order(me.Priority, other.Priority)}.";
    }

    private static string Order(int mine, int theirs)
    {
        if (mine == theirs)
            return "at the same priority, so which one wins is not defined";

        return theirs > mine ? "which runs later and wins" : "which runs earlier";
    }

    private static void WarnRetrigger(CheckContext context, List<Member> members, Member me, string field, Location at)
    {
        var triggered = members.Where(m => m.Table == me.Table && m.Event == rs_event.Update
            && (m.Filtered.Count == 0 || m.Filtered.Contains(field))).Select(m => m.Label).Distinct(StringComparer.Ordinal).ToList();
        if (triggered.Count > 0)
        {
            context.Add(CheckContext.Warning, ValidationCodes.RetriggerChain, $"Setting {field} after save updates the record again, which triggers {string.Join(", ", triggered)}.", at);
        }
    }

    private List<Member> Members(ReleaseInput input)
    {
        var members = input.Drafts.Select(d => new Member(d.Version, $"{d.RuleKey} (draft)", d.Actions)).ToList();
        var tables = members.Select(m => m.Table).Where(t => t.Length > 0).Distinct(StringComparer.Ordinal).ToArray();
        if (tables.Length == 0)
            return members;

        var releasing = new HashSet<Guid>(input.Drafts.Select(d => d.Rule.Id));
        var query = new QueryExpression(rs_RuleVersion.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition("rs_status", ConditionOperator.Equal, (int)rs_ruleversion_rs_status.Published);
        query.Criteria.AddCondition("rs_targettable", ConditionOperator.In, tables.Cast<object>().ToArray());
        var live = dao.RetrieveList<rs_RuleVersion>(query).Where(v => v.rs_RuleId != null && !releasing.Contains(v.rs_RuleId.Id)).ToList();
        var actions = dao.RetrieveMultipleByIds<rs_Action>(live.Select(v => v.Id), "rs_ruleversionid");
        members.AddRange(live.Select(v => new Member(v, v.rs_Name ?? v.Id.ToString(), actions.Where(a => a.rs_RuleVersionId?.Id == v.Id))));
        return members;
    }

    private sealed class Member
    {
        internal Member(rs_RuleVersion version, string label, IEnumerable<rs_Action> actions)
        {
            VersionId = version.Id;
            Label = label;
            Table = (version.rs_TargetTable ?? string.Empty).ToLowerInvariant();
            Event = version.rs_Event;
            Chain = $"{Table}|{version.rs_Event}|{version.rs_Stage}";
            Priority = version.rs_Priority ?? 0;
            StopProcessing = version.rs_StopProcessing == true;
            Filtered = new HashSet<string>(SnapshotCompiler.FilteredColumns(version), StringComparer.Ordinal);
            var setFields = actions
                .Where(a => a.rs_ActionType == rs_action_rs_actiontype.Setfield && a.rs_TargetField != null)
                .Select(a => a.rs_TargetField!.ToLowerInvariant());
            SetFields = new HashSet<string>(setFields, StringComparer.Ordinal);
        }

        internal Guid VersionId { get; }

        internal string Label { get; }

        internal string Table { get; }

        internal rs_event? Event { get; }

        internal string Chain { get; }

        internal int Priority { get; }

        internal bool StopProcessing { get; }

        internal HashSet<string> Filtered { get; }

        internal HashSet<string> SetFields { get; }
    }
}
