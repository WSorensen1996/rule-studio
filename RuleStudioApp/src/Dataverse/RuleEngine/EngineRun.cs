using System.Globalization;
using DataverseLogic.RuleStudio.Engine;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;

namespace RuleStudio.Engine;

/// <summary>One execution of the engine plug-in: load, run, apply, log (docs/engine.md).</summary>
internal sealed class EngineRun(IPluginExecutionContext context, IOrganizationServiceFactory factory, ITracingService tracing)
{
    private readonly IPluginExecutionContext context = context;
    private readonly IOrganizationServiceFactory factory = factory;
    private readonly ITracingService tracing = tracing;
    private IDataverseAccessObject? system;
    private IDataverseAccessObject? user;

    internal void Execute()
    {
        var trigger = PluginTrigger.From(context.MessageName, context.Stage);
        if (trigger == null)
        {
            tracing.Trace("Rule Studio: no rules run on {0} at stage {1}.", context.MessageName, context.Stage);
            return;
        }

        var table = context.PrimaryEntityName;
        if (context.Depth > EngineSettings.MaxDepth)
            throw new InvalidPluginExecutionException(DepthMessage(table));

        var target = context.InputParameters.Contains("Target") ? context.InputParameters["Target"] : null;
        var preImage = context.PreEntityImages.Contains(EngineSettings.PreImageName) ? context.PreEntityImages[EngineSettings.PreImageName] : null;
        var record = TriggerRecord.Build(table, context.PrimaryEntityId, target, preImage, trigger.Event);
        var loaded = RuleCatalog.Load(Dao(true), context.OrganizationId, table, trigger, DateTime.UtcNow);
        var rules = RuleSelection.Applicable(loaded, table, trigger, record.ChangedColumns);
        if (rules.Count == 0)
            return;

        var outcome = new ChainRunner(Dao, dao => new CustomApiInvoker(dao)).Run(record, trigger, rules);
        tracing.Trace("Rule Studio: {0} rules ran on {1} {2} {3}; correlation {4}.", outcome.Runs.Count, table, trigger.Event, trigger.Stage, context.CorrelationId);
        if (trigger.BeforeSave)
        {
            BeforeSave(trigger, table, target as Entity, outcome);
        }
        else
        {
            AfterSave(trigger, table, outcome);
        }
    }

    // The rules whose changes were not saved are failures for the log.
    private static void MarkApplyFailed(ChainOutcome outcome, string error)
    {
        var changed = outcome.Runs.Where(r => r.Trace.Actions.Any(a =>
            string.Equals(a.Type, "setField", StringComparison.Ordinal) && string.Equals(a.Status, "done", StringComparison.Ordinal)));
        foreach (var run in changed)
        {
            run.ApplyError = "The changes of the rule could not be saved: " + error;
            run.Trace.Outcome = "failed";
        }
    }

    private string DepthMessage(string table) => string.Format(
        CultureInfo.InvariantCulture,
        "Rule Studio stopped: rules on {0} triggered each other {1} levels deep (limit {2}). Check rules whose actions update records that trigger rules again.",
        table,
        context.Depth,
        EngineSettings.MaxDepth);

    private void BeforeSave(PluginTrigger trigger, string table, Entity? target, ChainOutcome outcome)
    {
        if (outcome.BlockMessage != null)
        {
            // Rows written now would be rolled back with the save, so the run goes to the plug-in trace log instead.
            foreach (var run in outcome.Runs)
            {
                tracing.Trace("Rule Studio: {0} v{1} {2} {3}", run.Rule.Rule.RuleKey, run.Rule.Rule.VersionNumber, run.Trace.Outcome, run.Trace.Error?.Message ?? string.Empty);
            }

            throw new InvalidPluginExecutionException(outcome.BlockMessage);
        }

        if (target != null && !string.Equals(trigger.Event, "delete", StringComparison.Ordinal))
        {
            ChangeApplier.ApplyToTarget(target, outcome);
        }

        new ExecutionLogWriter(Dao(true), tracing).Write(outcome, trigger, table, context.CorrelationId);
    }

    private void AfterSave(PluginTrigger trigger, string table, ChainOutcome outcome)
    {
        if (outcome.Changes.Count > 0 && !string.Equals(trigger.Event, "delete", StringComparison.Ordinal))
        {
            var error = ChangeApplier.UpdateRecord(table, context.PrimaryEntityId, outcome, Dao);
            if (error != null)
            {
                MarkApplyFailed(outcome, error);
            }
        }

        new ExecutionLogWriter(Dao(true), tracing).Write(outcome, trigger, table, context.CorrelationId);
    }

    // The system service (true) or the calling user's (false), created once.
    private IDataverseAccessObject Dao(bool asSystem)
    {
        if (asSystem)
            return system ??= new DataverseAccessObject(factory.CreateOrganizationService(null), NullLogger.Instance);

        return user ??= new DataverseAccessObject(factory.CreateOrganizationService(context.UserId), NullLogger.Instance);
    }
}
