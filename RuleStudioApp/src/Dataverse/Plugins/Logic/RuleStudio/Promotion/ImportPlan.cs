using Newtonsoft.Json.Linq;
using RuleStudio.SharedContext;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Promotion;

/// <summary>What an import found in the bundle and in this environment, and what it will change.</summary>
internal sealed class ImportPlan
{
    internal ImportReport Report { get; } = new();

    // This environment, by rule key, lookup name and Custom API name.
    internal Dictionary<string, rs_Rule> Rules { get; init; } = [];

    internal Dictionary<string, rs_LookupDefinition> Lookups { get; init; } = [];

    internal Dictionary<string, Guid> Functions { get; init; } = [];

    internal Dictionary<string, JObject> IncomingLookups { get; } = new(StringComparer.Ordinal);

    internal Dictionary<string, string> IncomingHashes { get; } = new(StringComparer.OrdinalIgnoreCase);

    // Rules to write as drafts and publish, in bundle order.
    internal List<(string Key, JObject Doc)> ToApply { get; } = [];
}
