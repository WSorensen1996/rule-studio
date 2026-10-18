using DataverseLogic;
using DataverseLogic.RuleStudio.Engine;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;

namespace RuleStudio.Engine;

/// <summary>
/// Runs a chain with the shared rule evaluator. Each rule reads, writes and calls functions with
/// its own Run as identity, so consecutive rules with the same identity are evaluated
/// together and the record, with the changes so far, is handed from one batch to the next.
/// </summary>
public sealed class ChainRunner
{
    private const string StoppedByEarlierRule = "stoppedByEarlierRule";

    private readonly Func<bool, IDataverseAccessObject> daoFor;
    private readonly Func<IDataverseAccessObject, IFunctionInvoker> invokerFor;

    // daoFor(true) is the system service, daoFor(false) the calling user's.
    public ChainRunner(Func<bool, IDataverseAccessObject> daoFor, Func<IDataverseAccessObject, IFunctionInvoker> invokerFor)
    {
        this.daoFor = daoFor;
        this.invokerFor = invokerFor;
    }

    // Runs the rules, which must already be in chain order (see RuleSelection).
    public ChainOutcome Run(TriggerRecord record, PluginTrigger trigger, IReadOnlyList<LiveRule> rules)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(record, nameof(record));
        ArgumentNullExceptionHelper.ThrowIfNull(trigger, nameof(trigger));
        ArgumentNullExceptionHelper.ThrowIfNull(rules, nameof(rules));
        var outcome = new ChainOutcome();
        var current = record.Record;
        var stopped = false;
        foreach (var batch in Batches(rules))
        {
            var request = new EvaluationRequest(current, trigger.Event, trigger.Stage)
            {
                PreImage = record.PreImage,
                ChangedColumns = record.ChangedColumns,
            };
            foreach (var compiled in batch.Select(r => r.Rule))
            {
                request.Rules.Add(compiled);
                if (stopped)
                {
                    request.Skip[compiled.RuleId] = StoppedByEarlierRule;
                }
            }

            var asSystem = batch[0].RunAsSystem;
            var dao = daoFor(asSystem);
            var result = new RuleEvaluator(dao, invokerFor(dao)).Evaluate(request);
            stopped = Collect(outcome, batch, result, asSystem) || stopped;
            current = WithChanges(current, result.Changes);
        }

        return outcome;
    }

    // Consecutive rules with the same identity.
    private static List<List<LiveRule>> Batches(IReadOnlyList<LiveRule> rules)
    {
        var batches = new List<List<LiveRule>>();
        foreach (var rule in rules)
        {
            if (batches.Count == 0 || batches[batches.Count - 1][0].RunAsSystem != rule.RunAsSystem)
            {
                batches.Add([]);
            }

            batches[batches.Count - 1].Add(rule);
        }

        return batches;
    }

    // Adds the batch's traces and changes; returns true when later rules must not run.
    private static bool Collect(ChainOutcome outcome, List<LiveRule> batch, EvaluationResult result, bool asSystem)
    {
        var stop = false;
        foreach (var trace in result.Traces)
        {
            var rule = batch.First(r => r.Rule.RuleId == trace.Rule?.RuleId);
            trace.Sequence = outcome.Runs.Count + 1;
            outcome.Runs.Add(new RuleRunRecord(rule, trace, result.Attempts.TryGetValue(rule.Rule.RuleId, out var attempts) ? attempts : 0));
            if (string.Equals(trace.Outcome, "matched", StringComparison.Ordinal) && rule.Rule.Runtime.StopProcessing)
            {
                stop = true;
            }
        }

        foreach (var change in result.Changes)
        {
            outcome.Changes[change.Key] = new FieldChange(change.Value, asSystem);
        }

        if (result.BlockMessage != null)
        {
            outcome.BlockMessage ??= result.BlockMessage;
            stop = true;
        }

        return stop;
    }

    private static Entity WithChanges(Entity record, IDictionary<string, object?> changes)
    {
        var next = new Entity(record.LogicalName, record.Id);
        foreach (var attribute in record.Attributes)
        {
            next[attribute.Key] = attribute.Value;
        }

        foreach (var change in changes)
        {
            next[change.Key] = change.Value;
        }

        return next;
    }
}
