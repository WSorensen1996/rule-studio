using DataverseLogic.RuleStudio.Release;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq;
using RuleStudio.SharedContext;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Promotion;

/// <summary>Reads this environment's rules and lookup definitions in their portable form.</summary>
internal sealed class PortableLoader(IDataverseAccessObject dao)
{
    private const int MaxRows = 5000;

    private readonly IDataverseAccessObject dao = dao;

    // Every active rule with a rule key, by key (case-insensitive, as the alternate key).
    internal Dictionary<string, rs_Rule> Rules()
    {
        var query = new QueryExpression(rs_Rule.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("rs_name", "rs_rulekey", "rs_description", "rs_enabled", "rs_publishedversionid", "rs_draftversionid", "rs_lastdeployedhash"),
            TopCount = MaxRows,
        };
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        query.Criteria.AddCondition("rs_rulekey", ConditionOperator.NotNull);
        return dao.RetrieveList<rs_Rule>(query)
            .GroupBy(r => r.rs_RuleKey!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
    }

    // The portable document of each rule's published version; rules without one are left out.
    internal Dictionary<Guid, JObject> Published(IEnumerable<rs_Rule> rules)
    {
        var list = rules.Where(r => r.rs_PublishedVersionId != null).ToList();
        var versions = dao.RetrieveMultipleByIds<rs_RuleVersion>(list.Select(r => r.rs_PublishedVersionId!.Id).Distinct(), v => v.rs_CompiledSnapshot)
            .ToDictionary(v => v.Id);
        var result = new Dictionary<Guid, JObject>();
        foreach (var rule in list)
        {
            if (!versions.TryGetValue(rule.rs_PublishedVersionId!.Id, out var version) || Json.Read<CompiledRule>(version.rs_CompiledSnapshot) is not { } compiled)
                continue;

            result[rule.Id] = PortableRule.FromSnapshot(compiled, rule);
        }

        return result;
    }

    // Every lookup definition by name (case-sensitive; the first wins when names repeat).
    internal Dictionary<string, rs_LookupDefinition> Lookups()
    {
        var query = new QueryExpression(rs_LookupDefinition.EntityLogicalName) { ColumnSet = new ColumnSet(true), TopCount = MaxRows };
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        return dao.RetrieveList<rs_LookupDefinition>(query)
            .Where(l => !string.IsNullOrEmpty(l.rs_Name))
            .GroupBy(l => l.rs_Name!, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);
    }

    // Custom functions by Custom API name.
    internal Dictionary<string, Guid> Functions()
    {
        var query = new QueryExpression(rs_CustomFunction.EntityLogicalName) { ColumnSet = new ColumnSet("rs_customapiname"), TopCount = MaxRows };
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        return dao.RetrieveList<rs_CustomFunction>(query)
            .Where(f => !string.IsNullOrEmpty(f.rs_CustomApiName))
            .GroupBy(f => f.rs_CustomApiName!, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.Ordinal);
    }

    // A lookup definition without its id: the compiled lookup plus its description.
    internal static JObject PortableLookup(rs_LookupDefinition lookup)
    {
        var doc = PortableRule.Parse(Json.Write(SnapshotCompiler.Lookup(lookup)));
        doc.Remove("id");
        doc["description"] = lookup.rs_Description;
        return doc;
    }
}
