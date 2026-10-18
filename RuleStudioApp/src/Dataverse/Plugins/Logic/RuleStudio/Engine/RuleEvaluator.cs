using System.Diagnostics;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>
/// The rule engine core: evaluates one table/event/stage chain of compiled rules against a record.
/// Rules run by priority, then rule key; each sees the changes of the rules
/// before it. It reads through the given access object, so its identity decides what it may read,
/// and never writes: the caller applies <see cref="EvaluationResult.Changes"/> and the block message.
/// </summary>
public sealed class RuleEvaluator(IDataverseAccessObject dao, IFunctionInvoker invoker)
{
    private const string Failed = "failed";

    private readonly IDataverseAccessObject dao = dao;
    private readonly IFunctionInvoker invoker = invoker;

    public EvaluationResult Evaluate(EvaluationRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        var context = new EngineContext(dao, invoker, request);
        var result = new EvaluationResult();
        var rules = request.Rules
            .OrderBy(r => r.Runtime.Priority)
            .ThenBy(r => r.RuleKey, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var stopped = false;
        foreach (var rule in rules)
        {
            var trace = NewTrace(context, rule, result.Traces.Count + 1);
            result.Traces.Add(trace);
            var skip = SkipReason(request, rule, stopped);
            if (skip != null)
            {
                trace.Outcome = "skipped";
                trace.SkipReason = skip;
                continue;
            }

            stopped = Run(context, rule, trace, result);
        }

        return result;
    }

    // Evaluates one lookup definition against the request's record (rs_SimulateRule's lookup test).
    public LookupTestResult TestLookup(EvaluationRequest request, CompiledLookup lookup)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        ArgumentNullExceptionHelper.ThrowIfNull(lookup, nameof(lookup));
        var context = new EngineContext(dao, invoker, request);
        var rule = new CompiledRule { RuleKey = "lookup-test" };
        rule.Lookups.Add(lookup);
        var run = new RuleRun(context, rule, new ExecutionTrace());
        try
        {
            return new LookupTestResult { Lookup = context.Lookups.Resolve(run, lookup).Lookup };
        }
        catch (EngineException ex)
        {
            return new LookupTestResult { Lookup = ex.Lookup, Error = new TraceError { Code = ex.Code, Message = ex.Message, Attempt = 1 } };
        }
    }

    private static string? SkipReason(EvaluationRequest request, CompiledRule rule, bool stopped)
    {
        if (stopped)
            return "stoppedByEarlierRule";

        if (request.Skip.TryGetValue(rule.RuleId, out var reason))
            return reason;

        var filtered = rule.Trigger.FilteredColumns;
        var changed = request.ChangedColumns;
        if (string.Equals(request.Event, "update", StringComparison.Ordinal) && filtered.Count > 0 && changed != null && !filtered.Any(changed.Contains))
            return "columnsNotChanged";

        return null;
    }

    private static ExecutionTrace NewTrace(EngineContext context, CompiledRule rule, int sequence)
    {
        var record = context.Record;
        var nameColumn = context.Metadata.Table(record.LogicalName)?.PrimaryNameAttribute;
        var now = DateTime.UtcNow;
        return new ExecutionTrace
        {
            Level = "fullTrace",
            Sequence = sequence,
            Rule = new TraceRule
            {
                RuleId = rule.RuleId,
                RuleKey = rule.RuleKey,
                RuleVersionId = rule.RuleVersionId,
                VersionNumber = rule.VersionNumber,
                Priority = rule.Runtime.Priority,
                RunAs = string.IsNullOrEmpty(rule.Runtime.RunAs) ? null : rule.Runtime.RunAs,
            },
            Record = new RecordReference
            {
                Table = record.LogicalName,
                Id = record.Id,
                Name = nameColumn != null && record.Attributes.TryGetValue(nameColumn, out var name) ? name as string : null,
            },
            StartedOn = new DateTime(now.Ticks - (now.Ticks % TimeSpan.TicksPerSecond), DateTimeKind.Utc),
            Test = context.Request.Simulation ? new TraceTest() : null,
        };
    }

    // Runs the rule with its retries; returns true when the chain must stop.
    private static bool Run(EngineContext context, CompiledRule rule, ExecutionTrace trace, EvaluationResult result)
    {
        var (retries, delay) = RetryLimits(context, rule.Runtime);
        var watch = Stopwatch.StartNew();
        for (var attempt = 1; ; attempt++)
        {
            Reset(trace);
            var run = new RuleRun(context, rule, trace);
            try
            {
                var stop = Execute(run);
                Commit(run, result);
                result.Attempts[rule.RuleId] = attempt;
                trace.DurationMs = (int)watch.ElapsedMilliseconds;
                return stop;
            }
#pragma warning disable CA1031 // Any failure of a rule is handled by its failure policy.
            catch (Exception ex)
#pragma warning restore CA1031
            {
                var engine = ex as EngineException;
                if (attempt <= retries && !run.CalledUnsafe && engine?.Blocks != true)
                {
                    Wait(context, delay);
                    continue;
                }

                Finish(run, result);
                result.Attempts[rule.RuleId] = attempt;
                trace.DurationMs = (int)watch.ElapsedMilliseconds;
                return Fail(context, run, ex, attempt, result);
            }
        }
    }

    private static (int Retries, int DelayMs) RetryLimits(EngineContext context, RuleRuntime runtime)
    {
        var maxRetries = context.BeforeSave ? EngineLimits.MaxBeforeSaveRetries : EngineLimits.MaxAfterSaveRetries;
        var maxDelay = context.BeforeSave ? EngineLimits.MaxBeforeSaveRetryDelayMs : EngineLimits.MaxAfterSaveRetryDelayMs;
        return (Math.Max(0, Math.Min(runtime.RetryCount, maxRetries)), Math.Max(0, Math.Min(runtime.RetryDelayMs, maxDelay)));
    }

    private static void Wait(EngineContext context, int delayMs)
    {
        // Test runs skip the delay: nothing is retried against a real failure worth waiting for.
        if (!context.Request.Simulation && delayMs > 0)
        {
            Thread.Sleep(delayMs);
        }
    }

    private static void Reset(ExecutionTrace trace)
    {
        trace.Conditions = null;
        trace.Branch = null;
        trace.Actions.Clear();
        trace.Error = null;
        trace.Test?.MockedFunctions.Clear();
    }

    private static bool Execute(RuleRun run)
    {
        var context = run.Context;
        var matched = context.Conditions.Evaluate(run, run.Rule.Conditions, node => run.Trace.Conditions = node);
        var actions = matched ? run.Rule.Then : run.Rule.Else;
        if (actions.Count == 0)
        {
            run.Trace.Branch = "none";
        }
        else
        {
            run.Trace.Branch = matched ? "then" : "else";
        }

        foreach (var action in actions)
        {
            context.Actions.Run(run, action);
        }

        if (run.BlockMessage != null)
        {
            run.Trace.Outcome = "blocked";
            return true;
        }

        run.Trace.Outcome = matched ? "matched" : "notMatched";
        return matched && run.Rule.Runtime.StopProcessing;
    }

    private static void Commit(RuleRun run, EvaluationResult result)
    {
        foreach (var change in run.Changes)
        {
            run.Context.Record[change.Key] = change.Value;
            result.Changes[change.Key] = change.Value;
        }

        if (run.BlockMessage != null)
        {
            result.BlockMessage = run.BlockMessage;
        }

        Finish(run, result);
    }

    // Records the function calls and mocks of the final attempt.
    private static void Finish(RuleRun run, EvaluationResult result)
    {
        foreach (var call in run.Functions)
        {
            result.FunctionCalls.Add(call);
        }

        foreach (var mocked in run.Mocked)
        {
            run.Trace.Test?.MockedFunctions.Add(mocked);
        }
    }

    // Applies the failure policy; returns true when the chain must stop.
    private static bool Fail(EngineContext context, RuleRun run, Exception ex, int attempt, EvaluationResult result)
    {
        var engine = ex as EngineException;
        var trace = run.Trace;
        trace.Outcome = engine?.Blocks == true ? "blocked" : Failed;
        trace.Error = new TraceError { Code = engine?.Code ?? EngineCodes.RuleFailed, Message = ex.Message, Attempt = attempt };
        AttachLookup(trace, engine);

        // After-save rules cannot block: their failures are always logged and processing continues.
        var blocks = context.BeforeSave && (engine?.Blocks == true || string.Equals(run.Rule.Runtime.FailurePolicy, "block", StringComparison.Ordinal));
        if (!blocks)
            return false;

        result.BlockMessage = $"{run.Rule.RuleKey}: {ex.Message}";
        return true;
    }

    // A lookup failure shows its evaluation on the action or condition that was running.
    private static void AttachLookup(ExecutionTrace trace, EngineException? engine)
    {
        if (engine?.Lookup == null)
            return;

        var action = trace.Actions.LastOrDefault();
        if (action != null && string.Equals(action.Status, Failed, StringComparison.Ordinal))
        {
            action.Lookup ??= engine.Lookup;
        }
    }
}
