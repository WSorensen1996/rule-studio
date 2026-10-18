using DataverseConnection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using RuleStudio.SharedContext;
using SharedContext.Dao;
using Tests.RuleStudio;

namespace LiveTests;

/// <summary>
/// A connection to the live environment and everything a test created there. Every record, rule and
/// release is named "TEST – live …" and removed again when the test class finishes.
/// </summary>
public sealed class LiveEnvironment : IDisposable
{
    internal const string Prefix = "TEST – live ";

    private readonly ServiceProvider? provider;
    private readonly List<Guid> releases = [];
    private readonly List<EntityReference> records = [];

    public LiveEnvironment()
    {
        if (string.IsNullOrWhiteSpace(Url))
            return;

        var credential = Environment.GetEnvironmentVariable("RULESTUDIO_LIVE_CREDENTIAL") ?? "browser";
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.Ordinal) { ["DataverseUrl"] = Url, ["DataverseCredentialType"] = credential }).Build());
        services.AddLogging();
        services.AddDataverseWithOrganizationServices(options => options.DataverseUrl = Url!);
        provider = services.BuildServiceProvider();
        Service = provider.GetRequiredService<IOrganizationService>();
        Dao = new DataverseAccessObject(Service, NullLogger.Instance);
        Rows = new RuleRows(Dao);

        // Leftovers of an interrupted earlier run would clash on rule keys.
        Purge();
    }

    internal static string? Url => Environment.GetEnvironmentVariable("RULESTUDIO_LIVE_URL");

    internal IOrganizationService Service { get; } = null!;

    internal IDataverseAccessObject Dao { get; } = null!;

    internal RuleRows Rows { get; } = null!;

    // A new rule (named TEST – live <key>) with a draft on account; returns rule and version ids.
    internal (Guid Rule, Guid Version) Draft(string key, rs_event @event, rs_stage stage, Action<rs_RuleVersion>? settings = null)
    {
        var rule = Rows.NewRule("LIVE." + key);
        Dao.Update(new rs_Rule(rule) { rs_Name = Prefix + key });
        return (rule, Rows.Draft(rule, "account", @event, stage, settings));
    }

    internal void Publish(params Guid[] ruleIds)
    {
        var response = Service.Execute(new OrganizationRequest("rs_PublishRelease")
        {
            ["RuleIds"] = JsonConvert.SerializeObject(ruleIds),
            ["Name"] = Prefix + "release",
        });
        var releaseId = (Guid)response["ReleaseId"];
        if (releaseId == Guid.Empty)
            throw new InvalidOperationException("Publish failed: " + response["ValidationResult"]);

        releases.Add(releaseId);
    }

    internal Guid Create(Entity record)
    {
        var id = Service.Create(record);
        records.Add(new EntityReference(record.LogicalName, id));
        return id;
    }

    internal Entity Read(string table, Guid id, params string[] columns) => Service.Retrieve(table, id, new ColumnSet(columns));

    // The engine caches a chain for 30 seconds, so a rule just published may not run on the first save.
    // Repeats the attempt until it returns true or the time is up.
    internal static bool Eventually(Func<bool> attempt, int seconds = 45)
    {
        var until = DateTime.UtcNow.AddSeconds(seconds);
        while (true)
        {
            if (attempt())
                return true;

            if (DateTime.UtcNow > until)
                return false;

            Thread.Sleep(3000);
        }
    }

    public void Dispose()
    {
        if (provider == null)
            return;

        // Roll the releases back newest first (the rules stop running), then delete what was created.
        foreach (var release in Enumerable.Reverse(releases))
        {
            Try(() => Service.Execute(new OrganizationRequest("rs_RollbackRelease") { ["ReleaseId"] = release }));
        }

        foreach (var record in Enumerable.Reverse(records))
        {
            Try(() => Service.Delete(record.LogicalName, record.Id));
        }

        Purge();
        Try(() => Service.Execute(new OrganizationRequest("rs_ReconcileSteps")));
        provider.Dispose();
    }

    // Deletes every "TEST – live" rule with its rows bottom-up (as TestData clean does), its logs and
    // the releases, including anything an earlier interrupted run left behind.
    private void Purge()
    {
        var rules = Named(rs_Rule.EntityLogicalName, "rs_name").Select(r => r.Id).ToList();
        foreach (var rule in rules)
        {
            Try(() => Service.Update(new Entity(rs_Rule.EntityLogicalName, rule) { ["rs_publishedversionid"] = null, ["rs_draftversionid"] = null }));
        }

        var versions = ByLookup("rs_ruleversion", "rs_ruleid", rules);
        var groups = ByLookup("rs_conditiongroup", "rs_ruleversionid", versions);
        var conditions = ByLookup("rs_condition", "rs_conditiongroupid", groups);
        var actions = ByLookup("rs_action", "rs_ruleversionid", versions);
        DeleteAll("rs_executionlog", ByLookup("rs_executionlog", "rs_ruleid", rules));
        DeleteAll("rs_actioninputmapping", ByLookup("rs_actioninputmapping", "rs_conditionid", conditions).Concat(ByLookup("rs_actioninputmapping", "rs_actionid", actions)));
        DeleteAll("rs_condition", conditions);
        DeleteAll("rs_action", actions);
        foreach (var group in groups)
        {
            Try(() => Service.Update(new Entity("rs_conditiongroup", group) { ["rs_parentgroupid"] = null }));
        }

        DeleteAll("rs_conditiongroup", groups);
        DeleteAll("rs_ruleversion", versions);
        DeleteAll(rs_Rule.EntityLogicalName, rules);
        var releaseRows = Named(rs_Release.EntityLogicalName, "rs_name").Concat(Named(rs_Release.EntityLogicalName, "rs_name", "Rollback of " + Prefix)).Select(r => r.Id).ToList();
        DeleteAll(rs_Release.EntityLogicalName, ByLookup(rs_Release.EntityLogicalName, "rs_rolledbackreleaseid", releaseRows).Concat(releaseRows).Distinct());
        DeleteAll("contact", Named("contact", "lastname").Select(c => c.Id));
        DeleteAll("account", Named("account", "name").Select(a => a.Id));
    }

    private List<Entity> Named(string table, string column, string prefix = Prefix)
    {
        var query = new QueryExpression(table) { ColumnSet = new ColumnSet(false) };
        query.Criteria.AddCondition(column, ConditionOperator.BeginsWith, prefix);
        return Service.RetrieveMultiple(query).Entities.ToList();
    }

    private List<Guid> ByLookup(string table, string column, IEnumerable<Guid> ids)
    {
        var list = ids.Distinct().Cast<object>().ToArray();
        if (list.Length == 0)
            return [];

        var query = new QueryExpression(table) { ColumnSet = new ColumnSet(false) };
        query.Criteria.AddCondition(column, ConditionOperator.In, list);
        return Service.RetrieveMultiple(query).Entities.Select(e => e.Id).ToList();
    }

    private void DeleteAll(string table, IEnumerable<Guid> ids)
    {
        foreach (var id in ids.Distinct())
        {
            Try(() => Service.Delete(table, id));
        }
    }

    private static void Try(Action action)
    {
        try
        {
            action();
        }
#pragma warning disable CA1031 // Clean-up continues past a row that is already gone.
        catch (Exception ex)
#pragma warning restore CA1031
        {
            Console.WriteLine("Live clean-up skipped a step: " + ex.Message);
        }
    }
}
