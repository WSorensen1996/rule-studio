// Rule Studio test data for manual and live testing. Run from the RuleStudioApp folder:
//   dotnet run --project TestData -- seed    creates or updates the TEST – accounts, contacts, tier rows,
//                                            and the lookup definition (matched on name; safe to rerun)
//   dotnet run --project TestData -- clean   deletes only records whose name starts with "TEST –": execution
//                                            logs, rules with their versions and rows, releases,
//                                            lookups, contacts and accounts; then reconciles engine steps
using DataverseConnection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

var environment = ResolveDataverseUrl();
const string prefix = "TEST – ";
var mode = args.FirstOrDefault() ?? string.Empty;
if (mode is not ("seed" or "clean"))
{
    Console.WriteLine("Usage: dotnet run --project TestData -- seed|clean");
    return 1;
}

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.Ordinal) { ["DataverseUrl"] = environment, ["DataverseCredentialType"] = "devicecode" }).Build());
services.AddLogging();
services.AddDataverseWithOrganizationServices(options =>
{
    options.DataverseUrl = environment;
    options.CredentialType = DataverseCredentialType.DeviceCodeCredential;
});
using var provider = services.BuildServiceProvider();
var service = provider.GetRequiredService<IOrganizationService>();
var who = (WhoAmIResponse)service.Execute(new WhoAmIRequest());

if (mode == "seed")
{
    Seed();
}
else
{
    Clean();
}

return 0;

// Finds a row by name (or creates it) and sets the values; returns its id.
Guid Upsert(string table, string nameColumn, string name, Dictionary<string, object?> values)
{
    var query = new QueryExpression(table) { ColumnSet = new ColumnSet(false), TopCount = 1 };
    query.Criteria.AddCondition(nameColumn, ConditionOperator.Equal, name);
    var existing = service.RetrieveMultiple(query).Entities.FirstOrDefault();
    var entity = new Entity(table) { [nameColumn] = name };
    foreach (var value in values)
    {
        entity[value.Key] = value.Value;
    }

    if (existing == null)
    {
        var id = service.Create(entity);
        Console.WriteLine($"  created  {table} {name}");
        return id;
    }

    entity.Id = existing.Id;
    service.Update(entity);
    Console.WriteLine($"  updated  {table} {name}");
    return existing.Id;
}

void Seed()
{
    Upsert("account", "name", prefix + "Small Co", new() { ["numberofemployees"] = 10, ["creditlimit"] = new Money(50_000), ["industrycode"] = new OptionSetValue(1), ["creditonhold"] = false, ["telephone1"] = "555-0100" });
    Upsert("account", "name", prefix + "Big Co", new() { ["numberofemployees"] = 500, ["creditlimit"] = new Money(2_000_000), ["creditonhold"] = false, ["telephone1"] = "555-0101" });
    var onHold = Upsert("account", "name", prefix + "On Hold Co", new() { ["creditonhold"] = true, ["telephone1"] = null });
    var parent = Upsert("account", "name", prefix + "Parent Co", new() { ["numberofemployees"] = 50, ["creditonhold"] = false });

    // Lookup matrix rows (test plan §4): category, employee range [numberofemployees, sharesoutstanding), return creditlimit.
    Upsert("account", "name", prefix + "Tier A", new() { ["accountnumber"] = "TIER", ["accountcategorycode"] = new OptionSetValue(1), ["numberofemployees"] = 0, ["sharesoutstanding"] = 100, ["creditlimit"] = new Money(100_000) });
    Upsert("account", "name", prefix + "Tier B", new() { ["accountnumber"] = "TIER", ["accountcategorycode"] = new OptionSetValue(1), ["numberofemployees"] = 100, ["sharesoutstanding"] = null, ["creditlimit"] = new Money(500_000) });
    Upsert("account", "name", prefix + "Tier C", new() { ["accountnumber"] = "TIER", ["accountcategorycode"] = null, ["numberofemployees"] = null, ["sharesoutstanding"] = null, ["creditlimit"] = new Money(10_000) });

    var parentRef = new EntityReference("account", parent);
    Upsert("contact", "lastname", prefix + "Ann", new() { ["firstname"] = "Ann", ["jobtitle"] = "CEO", ["numberofchildren"] = 1, ["parentcustomerid"] = parentRef });
    Upsert("contact", "lastname", prefix + "Bob", new() { ["firstname"] = "Bob", ["jobtitle"] = "Sales", ["numberofchildren"] = 2, ["parentcustomerid"] = parentRef });
    Upsert("contact", "lastname", prefix + "Cid", new() { ["firstname"] = "Cid", ["jobtitle"] = "Support", ["numberofchildren"] = 3, ["parentcustomerid"] = parentRef });
    Upsert("contact", "lastname", prefix + "Orphan", new() { ["firstname"] = "Orphan", ["parentcustomerid"] = null });
    Upsert("contact", "lastname", prefix + "On hold child", new() { ["firstname"] = "Hal", ["parentcustomerid"] = new EntityReference("account", onHold) });

    const string criteria = """
        {"version":1,"recordTable":"account","criteria":[
          {"operator":"equals","column":"accountnumber","value":{"type":"fixed","value":"TIER"}},
          {"operator":"equals","column":"accountcategorycode","emptyMatchesAll":true,"value":{"type":"fieldPath","path":{"table":"account","hops":[],"column":"accountcategorycode"}}},
          {"operator":"range","minColumn":"numberofemployees","maxColumn":"sharesoutstanding","emptyMatchesAll":true,"value":{"type":"fieldPath","path":{"table":"account","hops":[],"column":"numberofemployees"}}}]}
        """;
    Upsert("rs_lookupdefinition", "rs_name", prefix + "credit tier", new()
    {
        ["rs_description"] = "Credit limit by category and employee range.",
        ["rs_lookuptable"] = "account",
        ["rs_returncolumn"] = "creditlimit",
        ["rs_sortcolumn"] = "creditlimit",
        ["rs_matchcriteria"] = criteria.Replace("\r", string.Empty, StringComparison.Ordinal).Replace("\n", string.Empty, StringComparison.Ordinal),
        ["rs_nomatchpolicy"] = new OptionSetValue(126250001),
        ["rs_defaultvalue"] = "1000",
        ["rs_multimatchpolicy"] = new OptionSetValue(126250000),
    });
    Console.WriteLine("Seed complete.");
}

List<Entity> Named(string table, string nameColumn, params string[] columns)
{
    var query = new QueryExpression(table) { ColumnSet = new ColumnSet(columns) };
    query.Criteria.AddCondition(nameColumn, ConditionOperator.BeginsWith, prefix);
    return service.RetrieveMultiple(query).Entities.ToList();
}

List<Entity> ByLookup(string table, string column, IEnumerable<Guid> ids)
{
    var list = ids.ToList();
    if (list.Count == 0)
        return [];

    var query = new QueryExpression(table) { ColumnSet = new ColumnSet(false) };
    query.Criteria.AddCondition(column, ConditionOperator.In, list.Cast<object>().ToArray());
    return service.RetrieveMultiple(query).Entities.ToList();
}

void Delete(IEnumerable<Entity> rows, string label)
{
    var count = 0;
    foreach (var row in rows)
    {
        service.Delete(row.LogicalName, row.Id);
        count++;
    }

    Console.WriteLine($"  deleted  {count} {label}");
}

void Clean()
{
    // Rules: clear their version pointers first, then delete rows bottom-up. Only TEST – rules.
    var rules = Named("rs_rule", "rs_name");
    var ruleIds = rules.Select(r => r.Id).ToList();
    foreach (var rule in rules)
    {
        service.Update(new Entity("rs_rule", rule.Id) { ["rs_publishedversionid"] = null, ["rs_draftversionid"] = null });
    }

    Delete(ByLookup("rs_executionlog", "rs_ruleid", ruleIds), "execution logs of TEST rules");
    Delete(Named("rs_executionlog", "rs_name"), "TEST execution logs");
    var versions = ByLookup("rs_ruleversion", "rs_ruleid", ruleIds);
    var versionIds = versions.Select(v => v.Id).ToList();
    var groups = ByLookup("rs_conditiongroup", "rs_ruleversionid", versionIds);
    var conditions = ByLookup("rs_condition", "rs_conditiongroupid", groups.Select(g => g.Id));
    var actions = ByLookup("rs_action", "rs_ruleversionid", versionIds);
    Delete(ByLookup("rs_actioninputmapping", "rs_conditionid", conditions.Select(c => c.Id)).Concat(ByLookup("rs_actioninputmapping", "rs_actionid", actions.Select(a => a.Id))), "input mappings");
    Delete(conditions, "conditions");
    Delete(actions, "actions");
    foreach (var group in groups)
    {
        service.Update(new Entity("rs_conditiongroup", group.Id) { ["rs_parentgroupid"] = null });
    }

    Delete(groups, "condition groups");
    var releaseIds = versions.Select(v => service.Retrieve("rs_ruleversion", v.Id, new ColumnSet("rs_releaseid")).GetAttributeValue<EntityReference>("rs_releaseid")?.Id)
        .OfType<Guid>().Distinct().ToList();
    Delete(versions, "rule versions");
    Delete(rules, "rules");

    // Releases of TEST rules (all releases they published) and TEST-named releases, rollback releases first.
    var releases = Named("rs_release", "rs_name").Select(r => r.Id).Concat(releaseIds).Distinct().ToList();
    var rollbacks = ByLookup("rs_release", "rs_rolledbackreleaseid", releases).Select(r => r.Id);
    Delete(rollbacks.Concat(releases).Distinct().Select(id => new Entity("rs_release", id)), "releases");
    Delete(Named("rs_lookupdefinition", "rs_name"), "lookup definitions");
    Delete(Named("contact", "lastname"), "contacts");
    Delete(Named("account", "name"), "accounts");
    var summary = service.Execute(new OrganizationRequest("rs_ReconcileSteps"))["Summary"];
    Console.WriteLine($"Clean complete. {summary}");
}

// The target environment: the DataverseUrl environment variable, else appsettings.local.json, else appsettings.json.
static string ResolveDataverseUrl()
{
    var fromEnvironment = Environment.GetEnvironmentVariable("DataverseUrl");
    if (!string.IsNullOrWhiteSpace(fromEnvironment))
        return fromEnvironment;
    foreach (var file in new[] { "appsettings.local.json", "appsettings.json" })
    {
        if (!File.Exists(file))
            continue;
        using var json = System.Text.Json.JsonDocument.Parse(File.ReadAllText(file));
        if (json.RootElement.TryGetProperty("DataverseUrl", out var url) && url.GetString() is { Length: > 0 } value && !value.Contains("yourorg", StringComparison.OrdinalIgnoreCase))
            return value;
    }

    throw new InvalidOperationException("Set DataverseUrl (environment variable or appsettings.local.json) to your Dataverse environment URL.");
}
