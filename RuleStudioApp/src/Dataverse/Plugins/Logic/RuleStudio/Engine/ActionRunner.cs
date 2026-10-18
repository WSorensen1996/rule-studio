using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>Performs then/else actions. Set-field changes are staged on the rule run.</summary>
internal sealed class ActionRunner(EngineContext context)
{
    private const string Done = "done";
    private const string Skipped = "skipped";

    private readonly EngineContext context = context;

    // Runs the action. Its trace is added first with status failed, so an exception leaves it failed.
    internal void Run(RuleRun run, CompiledAction action)
    {
        var trace = new ActionTrace { Id = action.Id, Type = action.Type, Status = "failed" };
        run.Trace.Actions.Add(trace);
        if (run.BlockMessage != null)
        {
            trace.Status = Skipped;
            return;
        }

        switch (action.Type)
        {
            case "setField":
                trace.Status = SetField(run, action, trace);
                break;
            case "blockSave":
                Block(run, action, trace);
                trace.Status = Done;
                break;
            case "callFunction":
                var resolved = context.Functions.CallMapped(run, ValueResolver.Function(run.Rule, action.Value?.CustomFunctionId), action.Inputs);
                trace.Function = resolved.Function;
                resolved.ThrowIfFailed();
                trace.Status = Done;
                break;
            case "logMessage":
                trace.Message = action.Message;
                trace.MessageLevel = action.MessageLevel ?? "info";
                trace.Status = Done;
                break;
            default:
                throw new EngineException(EngineCodes.OperatorNotSupported, $"Action type {action.Type} is not supported.");
        }
    }

    private string SetField(RuleRun run, CompiledAction action, ActionTrace trace)
    {
        var field = action.TargetField ?? throw new EngineException(EngineCodes.FieldNotFound, "A set-field action has no target field.");
        trace.Field = field;
        var resolved = context.Sources.Resolve(run, action.Value, action.Inputs);
        trace.Lookup = resolved.Lookup;
        trace.Function = resolved.Function;
        resolved.ThrowIfFailed();
        if (resolved.Skipped)
            return Skipped;

        var table = run.Rule.Trigger.Table;
        var column = context.Metadata.Column(table, field);
        if (column == null && context.Metadata.Table(table) != null)
            throw new EngineException(EngineCodes.FieldNotFound, $"{table}.{field} no longer exists.");

        var value = Coerce.ToColumn(resolved.Value, column);
        trace.Before = Values.ForTrace(run.Value(field));
        run.Changes[field] = value;
        trace.After = Values.ForTrace(Values.Normalize(value));
        return Done;
    }

    private void Block(RuleRun run, CompiledAction action, ActionTrace trace)
    {
        if (!context.BeforeSave)
            throw new EngineException(EngineCodes.BlockSaveAfterSave, "Block save only works in before-save rules; the record is already saved.");

        trace.Message = string.IsNullOrWhiteSpace(action.Message) ? $"Blocked by rule {run.Rule.RuleKey}." : action.Message;
        run.BlockMessage = trace.Message;
    }
}
