using DataverseLogic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using RuleStudio.SharedContext;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace RuleStudio.Engine;

/// <summary>
/// Loads the published versions of the enabled rules of one table/event/stage.
/// A loaded chain is reused for <see cref="EngineSettings.ChainCacheDuration"/>; after that the
/// version ids and snapshot hashes are read again, and only snapshots whose hash changed are parsed
/// again. The cache lives in the sandbox worker and is per organization.
/// </summary>
public static class RuleCatalog
{
    private const string RuleAlias = "rule";
    private const int MaxRules = 5000;
    private const int MaxSnapshots = 2000;

    private static readonly object Gate = new();
    private static readonly Dictionary<string, CachedChain> Chains = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, CompiledRule> Snapshots = new(StringComparer.Ordinal);

    // The live rules of the trigger, in no particular order.
    public static IReadOnlyList<LiveRule> Load(IDataverseAccessObject dao, Guid organizationId, string table, PluginTrigger trigger, DateTime utcNow)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(dao, nameof(dao));
        ArgumentNullExceptionHelper.ThrowIfNull(trigger, nameof(trigger));
        var key = $"{organizationId}|{table}|{trigger.Event}|{trigger.Stage}";
        lock (Gate)
        {
            if (Chains.TryGetValue(key, out var cached) && cached.LoadedOn + EngineSettings.ChainCacheDuration > utcNow)
                return cached.Rules;
        }

        var rows = Versions(dao, table, trigger);
        var rules = Resolve(dao, organizationId, rows);
        lock (Gate)
        {
            Chains[key] = new CachedChain(utcNow, rules);
        }

        return rules;
    }

    // Drops every cached chain and snapshot (tests; a new deployment also starts empty).
    public static void Clear()
    {
        lock (Gate)
        {
            Chains.Clear();
            Snapshots.Clear();
        }
    }

    // The published version of each active, enabled rule, for the trigger.
    private static List<CatalogRow> Versions(IDataverseAccessObject dao, string table, PluginTrigger trigger)
    {
        var query = new QueryExpression(rs_RuleVersion.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("rs_ruleversionid", "rs_snapshothash"),
            TopCount = MaxRules,
        };
        query.Criteria.AddCondition("rs_status", ConditionOperator.Equal, (int)rs_ruleversion_rs_status.Published);
        query.Criteria.AddCondition("rs_targettable", ConditionOperator.Equal, table);
        query.Criteria.AddCondition("rs_event", ConditionOperator.Equal, (int)EventChoice(trigger.Event));
        query.Criteria.AddCondition("rs_stage", ConditionOperator.Equal, (int)(trigger.BeforeSave ? rs_stage.Beforesave : rs_stage.Aftersave));
        query.Criteria.AddCondition("rs_compiledsnapshot", ConditionOperator.NotNull);
        var rule = query.AddLink(rs_Rule.EntityLogicalName, "rs_ruleversionid", "rs_publishedversionid", JoinOperator.Inner);
        rule.EntityAlias = RuleAlias;
        rule.Columns = new ColumnSet("rs_enabled");
        rule.LinkCriteria.AddCondition("statecode", ConditionOperator.Equal, 0);

        return dao.RetrieveList<rs_RuleVersion>(query)
            .Where(v => Aliased<bool?>(v, RuleAlias, "rs_enabled") != false)
            .Select(v => new CatalogRow(v.Id, v.rs_SnapshotHash ?? string.Empty))
            .ToList();
    }

    // Each row's compiled rule: from the snapshot cache when its hash is known, else read and parsed.
    private static List<LiveRule> Resolve(IDataverseAccessObject dao, Guid organizationId, List<CatalogRow> rows)
    {
        var missing = Missing(organizationId, rows);
        if (missing.Count > 0)
        {
            var read = dao.RetrieveMultipleByIds<rs_RuleVersion>(missing, v => v.rs_CompiledSnapshot!, v => v.rs_SnapshotHash!);
            lock (Gate)
            {
                if (Snapshots.Count > MaxSnapshots)
                {
                    Snapshots.Clear();
                }

                foreach (var version in read)
                {
                    var compiled = TraceJson.ReadSnapshot(version.rs_CompiledSnapshot);
                    if (compiled != null)
                    {
                        Snapshots[$"{organizationId}|{version.Id}|{version.rs_SnapshotHash}"] = compiled;
                    }
                }
            }
        }

        var rules = new List<LiveRule>();
        lock (Gate)
        {
            foreach (var row in rows)
            {
                if (Snapshots.TryGetValue(SnapshotKey(organizationId, row), out var compiled))
                {
                    rules.Add(new LiveRule(compiled));
                }
            }
        }

        return rules;
    }

    // Versions whose snapshot (by hash) is not cached yet.
    private static List<Guid> Missing(Guid organizationId, List<CatalogRow> rows)
    {
        lock (Gate)
        {
            return rows.Where(r => !Snapshots.ContainsKey(SnapshotKey(organizationId, r))).Select(r => r.VersionId).ToList();
        }
    }

    private static string SnapshotKey(Guid organizationId, CatalogRow row) => $"{organizationId}|{row.VersionId}|{row.Hash}";

    private static T? Aliased<T>(Entity entity, string alias, string column)
    {
        var value = entity.GetAttributeValue<AliasedValue>($"{alias}.{column}")?.Value;
        return value is T typed ? typed : default;
    }

    private static rs_event EventChoice(string name) => name switch
    {
        "create" => rs_event.Create,
        "delete" => rs_event.Delete,
        _ => rs_event.Update,
    };
}
