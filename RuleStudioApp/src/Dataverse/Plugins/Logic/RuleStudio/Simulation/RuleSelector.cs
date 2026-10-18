using DataverseLogic.RuleStudio.Release;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using RuleStudio.SharedContext;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Simulation;

/// <summary>
/// Picks the compiled rules of a test run: drafts are compiled on the fly with the publish
/// compiler, published versions are read from their rs_compiledsnapshot.
/// </summary>
internal sealed class RuleSelector(IDataverseAccessObject dao)
{
    private const int MaxRules = 5000;

    private readonly IDataverseAccessObject dao = dao;

    // The selected rules; they must share one trigger. Disabled rules still run: they were picked on purpose.
    internal SimulatedChain ByIds(IReadOnlyCollection<Guid> ruleIds, bool useDrafts)
    {
        var rules = dao.RetrieveMultipleByIds<rs_Rule>(ruleIds);
        var missing = ruleIds.Except(rules.Select(r => r.Id)).ToList();
        if (missing.Count > 0)
            throw new InvalidPluginExecutionException($"Rule {missing[0]} no longer exists.");

        var chain = new SimulatedChain();
        foreach (var (rule, compiled) in Compile(rules, useDrafts, chain))
        {
            if (compiled == null)
                throw new InvalidPluginExecutionException($"{Name(rule)} has no draft or published version to test.");

            Add(chain, rule, compiled);
        }

        var first = chain.Rules[0].Trigger;
        chain.Table = first.Table;
        chain.Event = first.Event;
        chain.Stage = first.Stage;
        var other = chain.Rules.Find(r => !SameTrigger(r.Trigger, chain.Table, chain.Event, chain.Stage));
        if (other != null)
            throw new InvalidPluginExecutionException($"{chain.Names[other.RuleId]} runs on {Describe(other.Trigger)}, but {chain.Names[chain.Rules[0].RuleId]} runs on {Describe(first)}. Test rules of one table, event and stage together.");

        if (string.IsNullOrEmpty(chain.Table) || string.IsNullOrEmpty(chain.Event) || string.IsNullOrEmpty(chain.Stage))
            throw new InvalidPluginExecutionException($"{chain.Names[chain.Rules[0].RuleId]} has no complete trigger yet. Set its table, event and stage first.");

        return chain;
    }

    // Every rule whose evaluated version (draft where allowed, else published) uses the trigger.
    internal SimulatedChain ByTrigger(string table, string eventName, string stage, bool useDrafts)
    {
        var query = new QueryExpression(rs_Rule.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = MaxRules };
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        var rules = dao.RetrieveList<rs_Rule>(query);
        var chain = new SimulatedChain { Table = table, Event = eventName, Stage = stage };
        foreach (var (rule, compiled) in Compile(rules, useDrafts, chain))
        {
            if (compiled == null || !SameTrigger(compiled.Trigger, table, eventName, stage))
                continue;

            Add(chain, rule, compiled);
            if (rule.rs_Enabled == false)
            {
                chain.Skip[rule.Id] = "ruleDisabled";
            }
        }

        if (chain.Rules.Count == 0)
            throw new InvalidPluginExecutionException($"No {(useDrafts ? "draft or published" : "published")} rules run on {table} {eventName} {stage}.");

        return chain;
    }

    private static string Name(rs_Rule rule) => rule.rs_Name ?? rule.rs_RuleKey ?? rule.Id.ToString();

    private static void Add(SimulatedChain chain, rs_Rule rule, CompiledRule compiled)
    {
        chain.Rules.Add(compiled);
        chain.Names[rule.Id] = Name(rule);
    }

    private static bool SameTrigger(RuleTrigger trigger, string table, string eventName, string stage) =>
        string.Equals(trigger.Table, table, StringComparison.OrdinalIgnoreCase)
        && string.Equals(trigger.Event, eventName, StringComparison.Ordinal)
        && string.Equals(trigger.Stage, stage, StringComparison.Ordinal);

    private static string Describe(RuleTrigger trigger) => $"{trigger.Table} {trigger.Event} {trigger.Stage}";

    // Each rule with the version a test evaluates; null when it has neither a draft nor a snapshot.
    private List<(rs_Rule Rule, CompiledRule? Compiled)> Compile(List<rs_Rule> rules, bool useDrafts, SimulatedChain chain)
    {
        var drafts = new Dictionary<Guid, CompiledRule>();
        var withDraft = useDrafts ? rules.Where(r => r.rs_DraftVersionId != null).Select(r => r.Id).ToList() : [];
        if (withDraft.Count > 0)
        {
            var input = new DraftLoader(dao).Load(withDraft);
            foreach (var draft in input.Drafts)
            {
                drafts[draft.Rule.Id] = SnapshotCompiler.Compile(draft, input);
            }
        }

        var published = dao.RetrieveMultipleByIds<rs_RuleVersion>(
                rules.Where(r => !drafts.ContainsKey(r.Id) && r.rs_PublishedVersionId != null).Select(r => r.rs_PublishedVersionId!.Id))
            .ToDictionary(v => v.Id);
        var result = new List<(rs_Rule, CompiledRule?)>();
        foreach (var rule in rules)
        {
            if (drafts.TryGetValue(rule.Id, out var draft))
            {
                chain.Drafts.Add(rule.Id);
                result.Add((rule, draft));
                continue;
            }

            var version = rule.rs_PublishedVersionId != null && published.TryGetValue(rule.rs_PublishedVersionId.Id, out var found) ? found : null;
            result.Add((rule, Json.Read<CompiledRule>(version?.rs_CompiledSnapshot)));
        }

        return result;
    }
}
