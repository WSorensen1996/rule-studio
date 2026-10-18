using System.Diagnostics;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>
/// Calls custom functions. In test runs, functions with side effects are always mocked,
/// and the others too when the request mocks all functions.
/// </summary>
internal sealed class FunctionRunner(EngineContext context)
{
    internal const string MockedMessage = "Mocked in test mode.";

    private readonly EngineContext context = context;

    // Calls the function with inputs mapped from value sources.
    internal ResolvedValue CallMapped(RuleRun run, CompiledFunction function, IList<CompiledInput> inputs)
    {
        var values = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var input in inputs)
        {
            values[input.Name] = context.Sources.Resolve(run, input.Value, []).Value;
        }

        var trace = Call(run, function, values);
        return new ResolvedValue { Value = trace.Result.Success ? Values.Normalize(trace.Result.Value) : null, Function = trace };
    }

    internal FunctionTrace Call(RuleRun run, CompiledFunction function, IDictionary<string, object?> inputs)
    {
        var trace = new FunctionTrace { CustomFunctionId = function.Id, CustomApiName = function.ApiName };
        foreach (var input in inputs)
        {
            trace.Inputs[input.Key] = Values.ForTrace(input.Value);
        }

        var request = context.Request;
        var watch = Stopwatch.StartNew();
        if (request.Simulation && (function.HasSideEffects || request.MockAllFunctions))
        {
            trace.Mocked = true;
            trace.Result = new FunctionResult { Success = true, Message = MockedMessage };
            run.Mocked.Add(function.ApiName);
        }
        else
        {
            run.CalledUnsafe |= !function.SafeToRetry;
            trace.Result = Invoke(function, inputs);
            trace.Result.Value = Values.ForTrace(Values.Normalize(trace.Result.Value));
        }

        trace.DurationMs = (int)watch.ElapsedMilliseconds;

        // A synchronous call cannot be stopped halfway: one that took longer than its timeout counts as failed and its result is discarded.
        if (!trace.Mocked && function.TimeoutMs is > 0 && trace.DurationMs > function.TimeoutMs)
        {
            trace.TimedOut = true;
            trace.Result = new FunctionResult { Success = false, Message = $"took {trace.DurationMs} ms; its timeout is {function.TimeoutMs} ms" };
        }

        run.Functions.Add(trace);
        return trace;
    }

    private FunctionResult Invoke(CompiledFunction function, IDictionary<string, object?> inputs)
    {
        try
        {
            return context.Invoker.Invoke(function, inputs);
        }
#pragma warning disable CA1031 // Any failure of the Custom API is the function's failure; the rule's failure policy decides.
        catch (Exception ex) when (ex is not EngineException)
#pragma warning restore CA1031
        {
            return new FunctionResult { Success = false, Message = ex.Message };
        }
    }
}
