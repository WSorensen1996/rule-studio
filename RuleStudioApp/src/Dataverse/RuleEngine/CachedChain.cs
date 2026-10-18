namespace RuleStudio.Engine;

/// <summary>The live rules of one table/event/stage and when <see cref="RuleCatalog"/> loaded them.</summary>
internal sealed class CachedChain(DateTime loadedOn, IReadOnlyList<LiveRule> rules)
{
    internal DateTime LoadedOn { get; } = loadedOn;

    internal IReadOnlyList<LiveRule> Rules { get; } = rules;
}
