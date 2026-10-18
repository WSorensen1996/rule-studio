using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>Evaluates value sources: fixed, fieldPath, expression, lookup and function (json-formats.md "Value source").</summary>
internal sealed class ValueResolver(EngineContext context)
{
    private readonly EngineContext context = context;

    internal ResolvedValue Resolve(RuleRun run, ValueSource? source, IList<CompiledInput> inputs)
    {
        if (source == null)
            return new ResolvedValue();

        return source.Type switch
        {
            "fixed" => new ResolvedValue { Value = Values.Normalize(source.Value) },
            "fieldPath" => new ResolvedValue { Value = context.Paths.Read(run, source.Path ?? throw new EngineException(EngineCodes.FieldNotFound, "A field path value has no path.")) },
            "expression" => new ResolvedValue { Value = context.Expressions.Evaluate(run, source.Expression) },
            "lookup" => context.Lookups.Resolve(run, Lookup(run.Rule, source.LookupDefinitionId)),
            "function" => context.Functions.CallMapped(run, Function(run.Rule, source.CustomFunctionId), inputs),
            _ => throw new EngineException(EngineCodes.OperatorNotSupported, $"Value source {source.Type} is not supported."),
        };
    }

    // A value that may only be fixed, a field path or an expression (lookup criteria, function inputs).
    internal object? Simple(RuleRun run, ValueSource? source) => Resolve(run, source, []).Value;

    internal static CompiledFunction Function(CompiledRule rule, Guid? id) =>
        rule.Functions.FirstOrDefault(f => f.Id == id)
        ?? throw new EngineException(EngineCodes.FunctionNotRegistered, $"Function {id} is not part of rule {rule.RuleKey}'s snapshot.");

    private static CompiledLookup Lookup(CompiledRule rule, Guid? id) =>
        rule.Lookups.FirstOrDefault(l => l.Id == id)
        ?? throw new EngineException(EngineCodes.LookupNotFound, $"Lookup {id} is not part of rule {rule.RuleKey}'s snapshot.");
}
