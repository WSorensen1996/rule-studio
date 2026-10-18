namespace RuleStudio.Engine;

/// <summary>Fixed limits and names of the engine plug-in (docs/engine.md).</summary>
public static class EngineSettings
{
    // The engine's own recursion guard: below the platform's depth limit of 8, so rules
    // that keep re-triggering each other stop with a readable message instead of a platform error.
    public const int MaxDepth = 6;

    // Pre-image alias the step reconciler registers for Update and Delete steps.
    public const string PreImageName = "PreImage";

    // Log level of rules without their own level.
    public const string DefaultLogLevel = "errorsOnly";

    // rs_trace is a 1,048,576 character memo; the engine stays under about 90% of it (json-formats.md §4).
    public const int MaxTraceLength = 943718;

    public const int MaxErrorLength = 4000;

    public const int MaxNameLength = 850;

    public const int MaxShortTextLength = 100;

    // How long a loaded chain is reused before the published versions are checked again.
    public static readonly TimeSpan ChainCacheDuration = TimeSpan.FromSeconds(30);
}
