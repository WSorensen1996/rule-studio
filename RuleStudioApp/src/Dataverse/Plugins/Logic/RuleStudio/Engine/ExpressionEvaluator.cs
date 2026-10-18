using Microsoft.Xrm.Sdk.Metadata;
using NCalc;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>
/// NCalc formulas. Field references are bracketed parameters with dotted lookup names,
/// [totalamount] or [customerid:account.rs_segment] (json-formats.md, Decisions 1); registered
/// custom functions are called by name, with arguments in input-schema order.
/// </summary>
internal sealed class ExpressionEvaluator(EngineContext context)
{
    // Built-in names ignore case (as the publish check does), and Round rounds half away from zero,
    // as Set field does. Text comparison in a formula ignores case too, like the equals operator.
    internal const EvaluateOptions Options = EvaluateOptions.IgnoreCase | EvaluateOptions.RoundAwayFromZero;

    // NCalc's own functions; they are never routed to a custom function.
    internal static readonly HashSet<string> BuiltIns = new(StringComparer.OrdinalIgnoreCase)
    {
        "abs", "acos", "asin", "atan", "ceiling", "cos", "exp", "floor", "ieeeremainder", "log", "log10",
        "max", "min", "pow", "round", "sign", "sin", "sqrt", "tan", "truncate", "if", "in",
    };

    private readonly EngineContext context = context;
    private readonly Dictionary<string, FieldPath> paths = new(StringComparer.OrdinalIgnoreCase);

    internal object? Evaluate(RuleRun run, string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new EngineException(EngineCodes.ExpressionFailed, "The expression is empty.");

        var expression = new Expression(text, Options);
        if (expression.HasErrors())
            throw new EngineException(EngineCodes.ExpressionFailed, $"{text} could not be parsed: {expression.Error}");

        expression.EvaluateParameter += (name, args) => args.Result = Values.ForExpression(context.Paths.Read(run, Path(run.Rule, name)));
        expression.EvaluateFunction += (name, args) => CallFunction(run, name, args);
        try
        {
            return Values.Normalize(expression.Evaluate());
        }
#pragma warning disable CA1031 // NCalc throws several exception types for evaluation errors; all mean the formula failed.
        catch (Exception ex) when (ex is not EngineException)
#pragma warning restore CA1031
        {
            throw new EngineException(EngineCodes.ExpressionFailed, $"{text} failed: {ex.Message}");
        }
    }

    // The field path a bracketed reference stands for; each segment before the last is a lookup.
    private FieldPath Path(CompiledRule rule, string reference)
    {
        if (paths.TryGetValue(reference, out var cached))
            return cached;

        var segments = reference.Split('.');
        var path = new FieldPath { Table = rule.Trigger.Table, Column = segments[segments.Length - 1].Trim() };
        var table = rule.Trigger.Table;
        for (var i = 0; i < segments.Length - 1; i++)
        {
            var parts = segments[i].Split(':');
            var attribute = parts[0].Trim();
            var target = parts.Length > 1 ? parts[1].Trim() : SingleTarget(table, attribute, reference);
            path.Hops.Add(new FieldPathHop { Kind = "parent", Attribute = attribute, Table = target });
            table = target;
        }

        paths[reference] = path;
        return path;
    }

    private string SingleTarget(string table, string attribute, string reference)
    {
        var targets = (context.Metadata.Column(table, attribute) as LookupAttributeMetadata)?.Targets ?? [];
        if (targets.Length != 1)
            throw new EngineException(EngineCodes.ExpressionFailed, $"[{reference}]: {table}.{attribute} must be a lookup to one table, or name the table as {attribute}:<table>.");

        return targets[0];
    }

    private void CallFunction(RuleRun run, string name, FunctionArgs args)
    {
        if (BuiltIns.Contains(name))
            return;

        var function = run.Rule.Functions.FirstOrDefault(f =>
                string.Equals(f.ApiName, name, StringComparison.OrdinalIgnoreCase) || string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase))
            ?? context.Invoker.Find(name)
            ?? throw new EngineException(EngineCodes.FunctionNotRegistered, $"{name}() is not a registered custom function.");
        var arguments = args.EvaluateParameters();
        var schema = function.InputSchema?.Parameters ?? [];
        var inputs = new Dictionary<string, object?>(StringComparer.Ordinal);
        for (var i = 0; i < arguments.Length && i < schema.Count; i++)
        {
            inputs[schema[i].Name] = Values.Normalize(arguments[i]);
        }

        var trace = context.Functions.Call(run, function, inputs);
        new ResolvedValue { Function = trace }.ThrowIfFailed();
        args.Result = Values.ForExpression(Values.Normalize(trace.Result.Value));
    }
}
