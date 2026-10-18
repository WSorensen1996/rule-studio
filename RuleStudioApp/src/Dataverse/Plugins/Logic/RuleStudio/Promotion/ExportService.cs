using DataverseLogic.RuleStudio.Release;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Promotion;

/// <summary>
/// rs_ExportRules (docs/custom-apis.md). Returns the published rules as a bundle of
/// portable documents (json-formats.md §6), sorted by rule key, with the lookup definitions they
/// use. Runs as the calling user and changes nothing.
/// </summary>
public class ExportService(IUserDataverseAccessObjectService dao, IPluginExecutionContext context)
{
    private readonly IUserDataverseAccessObjectService dao = dao;
    private readonly IPluginExecutionContext context = context;

    // ruleKeysJson: optional JSON array of rule keys; empty exports every published rule.
    public string Export(string? ruleKeysJson)
    {
        var keys = Keys(ruleKeysJson);
        var loader = new PortableLoader(dao);
        var rules = loader.Rules();
        var unknown = keys.Where(k => !rules.ContainsKey(k)).ToList();
        if (unknown.Count > 0)
            throw new InvalidPluginExecutionException($"No rule has the key {string.Join(", ", unknown)}.");

        var selected = rules.Values.Where(r => keys.Count == 0 || keys.Contains(r.rs_RuleKey!)).ToList();
        var published = loader.Published(selected);
        var documents = selected.Where(r => published.ContainsKey(r.Id))
            .OrderBy(r => r.rs_RuleKey, StringComparer.OrdinalIgnoreCase)
            .Select(r => published[r.Id])
            .ToList();

        var used = new SortedSet<string>(documents.SelectMany(d => PortableRule.References(d).Lookups), StringComparer.Ordinal);
        var lookups = loader.Lookups();
        var bundle = new JObject
        {
            ["version"] = FormatVersion.Current,
            ["exportedOn"] = ReleaseService.Now().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", System.Globalization.CultureInfo.InvariantCulture),
            ["source"] = context.OrganizationName,
            ["rules"] = new JArray(documents),
            ["lookups"] = new JArray(used.Where(lookups.ContainsKey).Select(name => PortableLoader.PortableLookup(lookups[name]))),
        };
        return bundle.ToString(Formatting.None);
    }

    private static HashSet<string> Keys(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var keys = JsonConvert.DeserializeObject<string[]>(json!) ?? [];
            return new HashSet<string>(keys.Where(k => !string.IsNullOrWhiteSpace(k)).Select(k => k.Trim()), StringComparer.OrdinalIgnoreCase);
        }
        catch (JsonException)
        {
            throw new InvalidPluginExecutionException("RuleKeys must be a JSON array of rule keys.");
        }
    }
}
