using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>
/// Evaluates the condition tree with short-circuiting: AND stops at the first false child, OR at
/// the first true one; the rest are traced as not evaluated. An empty group matches.
/// </summary>
internal sealed class ConditionEvaluator(EngineContext context)
{
    private readonly EngineContext context = context;

    // Evaluates the node; its trace is attached before evaluating, so a failure still shows how far it got.
    internal bool Evaluate(RuleRun run, CompiledNode node, Action<TraceNode> attach)
    {
        var trace = Describe(node);
        attach(trace);
        var result = string.Equals(node.Type, "group", StringComparison.Ordinal) ? Group(run, node, trace) : Condition(run, node, trace);
        trace.Result = result;
        return result;
    }

    private static bool Quantifier(string aggregate, int matches, int total) => aggregate switch
    {
        "any" => matches > 0,
        "noneOf" => matches == 0,
        _ => matches == total,
    };

    private static TraceNode Describe(CompiledNode node)
    {
        var trace = new TraceNode { Type = node.Type, Id = node.Id, Operator = node.Operator, Path = node.Path };
        if (!string.Equals(node.Aggregate ?? "none", "none", StringComparison.Ordinal))
        {
            trace.Aggregate = node.Aggregate;
        }

        return trace;
    }

    private static TraceNode NotEvaluated(CompiledNode node)
    {
        var trace = Describe(node);
        trace.NotEvaluated = true;
        foreach (var child in node.Children)
        {
            trace.Children.Add(NotEvaluated(child));
        }

        return trace;
    }

    private bool Group(RuleRun run, CompiledNode node, TraceNode trace)
    {
        var any = string.Equals(node.Operator, "or", StringComparison.Ordinal);
        bool? decided = null;
        foreach (var child in node.Children)
        {
            if (decided.HasValue)
            {
                trace.Children.Add(NotEvaluated(child));
                continue;
            }

            if (Evaluate(run, child, trace.Children.Add) == any)
            {
                decided = any;
            }
        }

        return decided ?? (!any || node.Children.Count == 0);
    }

    private bool Condition(RuleRun run, CompiledNode node, TraceNode trace)
    {
        var path = node.Path ?? throw new EngineException(EngineCodes.FieldNotFound, "A condition has no field selected.");
        object? expected = null;
        if (Operators.NeedsValue(node.Operator))
        {
            ResolvedValue resolved;
            try
            {
                resolved = context.Sources.Resolve(run, node.Value, node.Inputs);
            }
            catch (EngineException ex) when (ex.Lookup != null)
            {
                // A failing lookup (Block or Error policy) still shows its evaluation on the condition.
                trace.Lookup = ex.Lookup;
                throw;
            }

            trace.Lookup = resolved.Lookup;
            trace.Function = resolved.Function;
            resolved.ThrowIfFailed();
            if (resolved.Skipped)
                return false;

            expected = resolved.Value;
            trace.Expected = Values.ForTrace(expected);
        }

        var (actual, result) = Test(run, path, node, expected, trace);
        trace.Actual = Values.ForTrace(actual);
        return result;
    }

    private (object? Actual, bool Result) Test(RuleRun run, FieldPath path, CompiledNode node, object? expected, TraceNode trace)
    {
        var aggregate = node.Aggregate ?? "none";
        switch (aggregate)
        {
            case "none" when string.Equals(node.Operator, "changed", StringComparison.Ordinal):
                return (run.Value(path.Column), Changed(run, path));
            case "none":
                var actual = context.Paths.Read(run, path);
                return (actual, Operators.Test(node.Operator, actual, expected));
            case "any":
            case "all":
            case "noneOf":
                // All over no child records is true (nothing breaks the rule); the trace shows "0 of 0".
                var values = context.Paths.ChildValues(run, path);
                var matches = values.Count(v => Operators.Test(node.Operator, v, expected));
                trace.Total = values.Count;
                return ((decimal)matches, Quantifier(aggregate, matches, values.Count));
            default:
                var total = context.Paths.Aggregate(run, path, aggregate);
                return (total, Operators.Test(node.Operator, total, expected));
        }
    }

    // Update only: the column is among the saved columns and differs from the pre-image.
    private bool Changed(RuleRun run, FieldPath path)
    {
        var request = context.Request;
        if (!string.Equals(request.Event, "update", StringComparison.Ordinal) || path.Hops.Count > 0)
            return false;

        var listed = request.ChangedColumns?.Contains(path.Column);
        if (listed == false)
            return false;

        if (request.PreImage == null || !request.PreImage.Attributes.TryGetValue(path.Column, out var before))
            return listed == true;

        return !Values.AreEqual(Values.Normalize(before), run.Value(path.Column));
    }
}
