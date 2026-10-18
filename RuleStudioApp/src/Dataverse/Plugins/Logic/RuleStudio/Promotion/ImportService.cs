using DataverseLogic.RuleStudio.Release;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuleStudio.SharedContext;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Promotion;

/// <summary>
/// rs_ImportRules (docs/custom-apis.md). Compares each incoming rule with the
/// target's published version and the hash the last import deployed (three-way), then, in apply
/// mode and only when nothing conflicts, writes the changed rules as drafts and publishes them as
/// one release. Lookup definitions travel with the bundle and are matched by name; functions must
/// already exist (matched by Custom API name). Runs as the calling user in the Custom API's
/// transaction: a failed publish rolls everything back.
/// </summary>
public class ImportService(IUserDataverseAccessObjectService dao, ReleaseService releases)
{
    private const string Error = "error";
    private const string Warning = "warning";

    private readonly IUserDataverseAccessObjectService dao = dao;
    private readonly ReleaseService releases = releases;

    // mode: check (no changes) or apply. Returns the ImportReport as JSON.
    public string Import(string bundleJson, string mode)
    {
        var apply = string.Equals(mode, "apply", StringComparison.OrdinalIgnoreCase);
        if (!apply && !string.Equals(mode, "check", StringComparison.OrdinalIgnoreCase))
            throw new InvalidPluginExecutionException($"Mode must be check or apply, not {mode}.");

        var bundle = ParseBundle(bundleJson);
        var loader = new PortableLoader(dao);
        var plan = Plan(bundle, loader);
        plan.Report.Mode = apply ? "apply" : "check";
        var blocked = plan.Report.Rules.Any(r => r.Status == "conflict") || plan.Report.Issues.Any(i => i.Severity == Error);
        plan.Report.Outcome = blocked ? "blocked" : "clean";
        if (apply && !blocked)
        {
            Apply(plan, loader);
            plan.Report.Outcome = "applied";
        }

        return Json.Write(plan.Report);
    }

    // The three-way decision for one rule. current is null when the target has no published version.
    internal static string Classify(string incoming, string? current, string? lastDeployed)
    {
        if (string.Equals(current, incoming, StringComparison.Ordinal))
            return "unchanged";

        if (current == null && lastDeployed == null)
            return "create";

        if (string.Equals(current, lastDeployed, StringComparison.Ordinal))
            return "update";

        return string.Equals(incoming, lastDeployed, StringComparison.Ordinal) ? "keepTarget" : "conflict";
    }

    private static JObject ParseBundle(string text)
    {
        JObject bundle;
        try
        {
            bundle = PortableRule.Parse(text ?? string.Empty);
        }
        catch (JsonException ex)
        {
            throw new InvalidPluginExecutionException($"The bundle is not valid JSON: {ex.Message}");
        }

        if (bundle["version"]?.Type != JTokenType.Integer || (int)bundle["version"]! != FormatVersion.Current)
            throw new InvalidPluginExecutionException($"Unsupported bundle format: version {bundle["version"]} (expected {FormatVersion.Current}).");

        return bundle;
    }

    private static ValidationIssue Issue(string severity, string code, string message, string? ruleKey = null) =>
        new() { Severity = severity, Code = code, Message = message, RuleKey = ruleKey };

    private static string Message(string status) => status switch
    {
        "create" => "New in this environment: will be created and published.",
        "update" => "Unchanged here since the last import: the incoming version will be published.",
        "unchanged" => "Already identical.",
        "keepTarget" => "Changed only in this environment: kept. Back-merge it to Git.",
        _ => "Changed both here and in the source since the last import. Resolve the difference in Git, then run the check again.",
    };

    private static ImportPlan Plan(JObject bundle, PortableLoader loader)
    {
        var plan = new ImportPlan { Lookups = loader.Lookups(), Functions = loader.Functions(), Rules = loader.Rules() };
        plan.Report.Source = bundle["source"]?.ToString();
        PlanLookups(bundle, plan);
        var current = loader.Published(plan.Rules.Values);
        var incomingKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var doc in (bundle["rules"] as JArray)?.OfType<JObject>() ?? [])
        {
            var key = doc["ruleKey"]?.ToString();
            if (string.IsNullOrWhiteSpace(key) || !incomingKeys.Add(key!))
            {
                plan.Report.Issues.Add(Issue(Error, ValidationCodes.ImportInvalidRule, string.IsNullOrWhiteSpace(key) ? "A rule in the bundle has no ruleKey." : $"Rule key {key} appears twice in the bundle.", key));
                continue;
            }

            PlanRule(plan, doc, key!, current);
        }

        foreach (var rule in plan.Rules.Values.Where(r => !incomingKeys.Contains(r.rs_RuleKey!) && current.ContainsKey(r.Id)).OrderBy(r => r.rs_RuleKey, StringComparer.OrdinalIgnoreCase))
        {
            plan.Report.Rules.Add(new ImportRuleResult
            {
                RuleKey = rule.rs_RuleKey!,
                Name = rule.rs_Name,
                Status = "targetOnly",
                CurrentHash = current[rule.Id]["hash"]?.ToString(),
                BaseHash = rule.rs_LastDeployedHash,
                Message = "Exists only in this environment: kept. Back-merge it to Git.",
            });
        }

        return plan;
    }

    private static void PlanLookups(JObject bundle, ImportPlan plan)
    {
        foreach (var doc in (bundle["lookups"] as JArray)?.OfType<JObject>() ?? [])
        {
            var name = doc["name"]?.ToString();
            if (string.IsNullOrWhiteSpace(name))
            {
                plan.Report.Issues.Add(Issue(Error, ValidationCodes.ImportInvalidRule, "A lookup definition in the bundle has no name."));
                continue;
            }

            var status = "create";
            if (plan.Lookups.TryGetValue(name!, out var existing))
            {
                status = JToken.DeepEquals(PortableLoader.PortableLookup(existing), doc) ? "unchanged" : "update";
            }

            plan.IncomingLookups[name!] = doc;
            plan.Report.Lookups.Add(new ImportLookupResult { Name = name!, Status = status });
        }
    }

    private static void PlanRule(ImportPlan plan, JObject doc, string key, Dictionary<Guid, JObject> current)
    {
        var hash = PortableRule.Hash(doc);
        plan.Rules.TryGetValue(key, out var rule);
        var currentDoc = rule != null && current.TryGetValue(rule.Id, out var found) ? found : null;
        var status = Classify(hash, currentDoc?["hash"]?.ToString(), rule?.rs_LastDeployedHash);
        if (status == "create" && rule != null)
        {
            status = "update";
        }

        var result = new ImportRuleResult
        {
            RuleKey = key,
            Name = doc["name"]?.ToString(),
            Status = status,
            IncomingHash = hash,
            CurrentHash = currentDoc?["hash"]?.ToString(),
            BaseHash = rule?.rs_LastDeployedHash,
            Message = Message(status),
        };
        if (status == "conflict")
        {
            result.Incoming = doc;
            result.Current = currentDoc;
            plan.Report.Issues.Add(Issue(Error, ValidationCodes.ImportConflict, $"{key}: {Message(status)}", key));
        }

        plan.Report.Rules.Add(result);
        plan.IncomingHashes[key] = hash;
        if (status is "create" or "update")
        {
            plan.ToApply.Add((key, doc));
            CheckReferences(plan, doc, key);
            if (rule?.rs_DraftVersionId != null)
            {
                plan.Report.Issues.Add(Issue(Warning, ValidationCodes.ImportDraftDiscarded, $"{key}: the unpublished draft in this environment will be discarded (kept in history).", key));
            }
        }
    }

    private static void CheckReferences(ImportPlan plan, JObject doc, string key)
    {
        var (lookups, functions) = PortableRule.References(doc);
        foreach (var lookup in lookups.Where(l => !plan.IncomingLookups.ContainsKey(l) && !plan.Lookups.ContainsKey(l)))
        {
            plan.Report.Issues.Add(Issue(Error, ValidationCodes.ImportLookupMissing, $"{key} uses lookup definition \"{lookup}\", which is neither in the bundle nor in this environment.", key));
        }

        foreach (var function in functions.Where(f => !plan.Functions.ContainsKey(f)))
        {
            plan.Report.Issues.Add(Issue(Error, ValidationCodes.ImportFunctionMissing, $"{key} calls {function}, which is not registered as a custom function in this environment. Deploy and register it first.", key));
        }
    }

    private void Apply(ImportPlan plan, PortableLoader loader)
    {
        var lookupIds = ApplyLookups(plan);
        var ruleIds = new List<Guid>();
        foreach (var (key, doc) in plan.ToApply)
        {
            var missing = new List<string>();
            var compiled = PortableRule.ToCompiled(doc, lookupIds, plan.Functions, missing);
            if (missing.Count > 0)
                throw new InvalidPluginExecutionException($"{key}: {string.Join(", ", missing)} not found.");

            plan.Rules.TryGetValue(key, out var rule);
            ruleIds.Add(new DraftImporter(dao).Write(rule, key, doc, compiled, plan.IncomingHashes[key]));
        }

        if (ruleIds.Count > 0)
        {
            var name = $"Import {ReleaseService.Now():yyyy-MM-dd HH:mm} ({ruleIds.Count} rule{(ruleIds.Count == 1 ? string.Empty : "s")})";
            var (validation, releaseId) = releases.Publish(JsonConvert.SerializeObject(ruleIds), name, $"Imported from {plan.Report.Source ?? "a bundle"}.");
            if (releaseId == Guid.Empty)
                throw new InvalidPluginExecutionException($"The imported rules do not pass the publish checks; nothing was imported. {Failures(validation)}");

            plan.Report.ReleaseId = releaseId;
        }

        foreach (var key in plan.Report.Rules.Where(r => r.Status is "create" or "update" or "unchanged").Select(r => r.RuleKey))
        {
            var id = plan.Rules.TryGetValue(key, out var existing) ? existing.Id : ruleIds[plan.ToApply.FindIndex(a => string.Equals(a.Key, key, StringComparison.Ordinal))];
            dao.Update(new rs_Rule(id) { rs_LastDeployedHash = plan.IncomingHashes[key] });
        }

        CheckRoundTrip(plan, loader);
    }

    private Dictionary<string, Guid> ApplyLookups(ImportPlan plan)
    {
        var ids = plan.Lookups.ToDictionary(l => l.Key, l => l.Value.Id, StringComparer.Ordinal);
        foreach (var result in plan.Report.Lookups.Where(l => l.Status != "unchanged"))
        {
            var doc = plan.IncomingLookups[result.Name];
            var lookup = doc.ToObject<CompiledLookup>() ?? new CompiledLookup();
            var row = new rs_LookupDefinition
            {
                rs_Name = result.Name,
                rs_Description = doc["description"]?.Type == JTokenType.String ? doc["description"]!.ToString() : null,
                rs_LookupTable = lookup.Table,
                rs_ReturnColumn = lookup.ReturnColumn,
                rs_SortColumn = lookup.SortColumn,
                rs_NoMatchPolicy = ChoiceValues.Parse<rs_lookupdefinition_rs_nomatchpolicy>(lookup.NoMatch, ChoiceNames.NoMatch),
                rs_MultiMatchPolicy = ChoiceValues.Parse<rs_lookupdefinition_rs_multimatchpolicy>(lookup.MultiMatch, ChoiceNames.MultiMatch),
                rs_DefaultValue = doc["defaultValue"] is { } value ? value.ToString(Formatting.None) : null,
                rs_MatchCriteria = lookup.MatchCriteria == null ? null : Json.Write(lookup.MatchCriteria),
            };
            if (result.Status == "update")
            {
                row.Id = ids[result.Name];
                dao.Update(row);
            }
            else
            {
                ids[result.Name] = dao.Create(row);
            }
        }

        return ids;
    }

    private static string Failures(string validation)
    {
        var result = Json.Read<ValidationResult>(validation);
        var errors = result?.Issues.Where(i => i.Severity == Error).Select(i => $"{i.RuleKey}: {i.Message}").Take(10) ?? [];
        return string.Join(" ", errors);
    }

    // The published result must hash like the incoming rule, or the next import would see a change here.
    private static void CheckRoundTrip(ImportPlan plan, PortableLoader loader)
    {
        var rules = loader.Rules();
        var published = loader.Published(plan.ToApply.Select(a => rules[a.Key]));
        foreach (var (key, _) in plan.ToApply)
        {
            var hash = published.TryGetValue(rules[key].Id, out var doc) ? doc["hash"]?.ToString() : null;
            if (!string.Equals(hash, plan.IncomingHashes[key], StringComparison.Ordinal))
            {
                plan.Report.Issues.Add(Issue(Warning, ValidationCodes.ImportInvalidRule, $"{key}: the published version does not hash like the incoming rule; the next import will report it as changed here.", key));
            }
        }
    }
}
