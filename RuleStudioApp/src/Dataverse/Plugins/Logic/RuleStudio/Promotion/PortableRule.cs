using DataverseLogic.RuleStudio.Release;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuleStudio.SharedContext;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Promotion;

/// <summary>
/// The portable rule document (json-formats.md §6): a published snapshot without environment ids.
/// Lookups are referenced by name and functions by Custom API name, so the same rule hashes the
/// same in every environment.
/// </summary>
internal static class PortableRule
{
    private static readonly JsonSerializerSettings Settings = new() { DateParseHandling = DateParseHandling.None };

    // The parts of a snapshot that differ per environment.
    private static readonly string[] EnvironmentProperties = ["ruleId", "ruleVersionId", "versionNumber", "lookups", "functions"];

    internal static JObject FromSnapshot(CompiledRule compiled, rs_Rule rule)
    {
        var lookups = compiled.Lookups.GroupBy(l => l.Id).ToDictionary(g => g.Key, g => g.First().Name);
        var functions = compiled.Functions.GroupBy(f => f.Id).ToDictionary(g => g.Key, g => g.First().ApiName);
        var snapshot = Parse(Json.Write(compiled));
        foreach (var name in EnvironmentProperties)
        {
            snapshot.Remove(name);
        }

        Walk(snapshot, source => ToNames(source, lookups, functions));
        var doc = new JObject
        {
            ["version"] = FormatVersion.Current,
            ["ruleKey"] = rule.rs_RuleKey ?? string.Empty,
            ["name"] = rule.rs_Name ?? string.Empty,
            ["description"] = rule.rs_Description,
            ["enabled"] = rule.rs_Enabled != false,
        };
        foreach (var property in snapshot.Properties().Where(p => !string.Equals(p.Name, "version", StringComparison.Ordinal)).ToList())
        {
            doc[property.Name] = property.Value;
        }

        doc["hash"] = Hash(doc);
        return doc;
    }

    // SHA-256 of the document without its hash and enabled flag (enabling is per environment).
    internal static string Hash(JObject doc)
    {
        var copy = (JObject)doc.DeepClone();
        copy.Remove("hash");
        copy.Remove("enabled");
        return Json.Hash(copy.ToString(Formatting.None));
    }

    internal static JObject Parse(string text) =>
        JsonConvert.DeserializeObject<JObject>(text, Settings) ?? throw new JsonException("The document is empty.");

    // The compiled rule with lookup and function names resolved to this environment's ids; unknown names go to missing.
    internal static CompiledRule ToCompiled(JObject doc, IDictionary<string, Guid> lookupIds, IDictionary<string, Guid> functionIds, ICollection<string> missing)
    {
        var copy = (JObject)doc.DeepClone();
        Walk(copy, source => ToIds(source, lookupIds, functionIds, missing));
        return copy.ToObject<CompiledRule>(JsonSerializer.Create(Settings)) ?? new CompiledRule();
    }

    // Lookup names and function Custom API names the document uses.
    internal static (HashSet<string> Lookups, HashSet<string> Functions) References(JObject doc)
    {
        var lookups = new HashSet<string>(StringComparer.Ordinal);
        var functions = new HashSet<string>(StringComparer.Ordinal);
        Walk((JObject)doc.DeepClone(), source =>
        {
            if (source["lookup"] is JValue { Value: string lookup })
            {
                lookups.Add(lookup);
            }

            if (source["function"] is JValue { Value: string function })
            {
                functions.Add(function);
            }
        });
        return (lookups, functions);
    }

    // Visits every value source of the condition tree and the actions, and drops the node and action ids.
    private static void Walk(JObject doc, Action<JObject> visit)
    {
        if (doc["conditions"] is JObject root)
        {
            WalkNode(root, visit);
        }

        foreach (var branch in new[] { "then", "else" })
        {
            foreach (var action in (doc[branch] as JArray)?.OfType<JObject>() ?? [])
            {
                action.Remove("id");
                Visit(action, visit);
            }
        }
    }

    private static void WalkNode(JObject node, Action<JObject> visit)
    {
        node.Remove("id");
        Visit(node, visit);
        foreach (var child in (node["children"] as JArray)?.OfType<JObject>() ?? [])
        {
            WalkNode(child, visit);
        }
    }

    private static void Visit(JObject owner, Action<JObject> visit)
    {
        if (owner["value"] is JObject source && source["type"] != null)
        {
            visit(source);
        }

        foreach (var input in (owner["inputs"] as JArray)?.OfType<JObject>() ?? [])
        {
            if (input["value"] is JObject inputSource)
            {
                visit(inputSource);
            }
        }
    }

    private static void ToNames(JObject source, Dictionary<Guid, string> lookups, Dictionary<Guid, string> functions)
    {
        if (source["lookupDefinitionId"] is JToken lookup)
        {
            source.Remove("lookupDefinitionId");
            source["lookup"] = Guid.TryParse(lookup.ToString(), out var id) && lookups.TryGetValue(id, out var name) ? name : lookup.ToString();
        }

        if (source["customFunctionId"] is JToken function)
        {
            source.Remove("customFunctionId");
            source["function"] = Guid.TryParse(function.ToString(), out var id) && functions.TryGetValue(id, out var api) ? api : function.ToString();
        }
    }

    private static void ToIds(JObject source, IDictionary<string, Guid> lookupIds, IDictionary<string, Guid> functionIds, ICollection<string> missing)
    {
        if (source["lookup"] is JToken lookup)
        {
            source.Remove("lookup");
            if (lookupIds.TryGetValue(lookup.ToString(), out var id))
            {
                source["lookupDefinitionId"] = id;
            }
            else
            {
                missing.Add($"lookup {lookup}");
            }
        }

        if (source["function"] is JToken function)
        {
            source.Remove("function");
            if (functionIds.TryGetValue(function.ToString(), out var id))
            {
                source["customFunctionId"] = id;
            }
            else
            {
                missing.Add($"function {function}");
            }
        }
    }
}
