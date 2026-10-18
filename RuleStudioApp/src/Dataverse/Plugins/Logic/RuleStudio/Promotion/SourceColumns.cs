using Microsoft.Xrm.Sdk;
using RuleStudio.SharedContext;

namespace DataverseLogic.RuleStudio.Promotion;

/// <summary>The value-source columns of a condition, action or input mapping row.</summary>
internal sealed class SourceColumns
{
    internal rs_valuesourcetype? Type { get; init; }

    internal string? Value { get; init; }

    internal string? Path { get; init; }

    internal EntityReference? Lookup { get; init; }

    internal EntityReference? Function { get; init; }
}
