// Creates the Rule Studio Author and Viewer security roles in the RuleStudio solution, grants their
// privileges on the 10 rs_ tables, and links both roles to the app in the RuleStudio solution
// (run DeployDataModel and DeployApp first). Idempotent.
// Run from the RuleStudioApp folder: dotnet run --project DeploySecurityRoles
using System.Text.Json;
using DataverseConnection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

var environment = ResolveDataverseUrl();
const string solution = "RuleStudio";
const int appModuleComponent = 80;
string[] tables = ["rs_rule", "rs_ruleversion", "rs_conditiongroup", "rs_condition", "rs_action", "rs_actioninputmapping", "rs_customfunction", "rs_lookupdefinition", "rs_release", "rs_executionlog"];
string[] authorVerbs = ["Create", "Read", "Write", "Delete", "Append", "AppendTo"];
string[] viewerVerbs = ["Read"];

// Custom API catalog: the register-function dialog lists Custom APIs and their parameters.
// Request parameters and response properties have no privileges of their own; prvReadCustomAPI covers them.
// rs_release.rs_publishedby and rs_ruleversion.rs_publishedby point at the publisher: publishing needs AppendTo on users.
string[] authorExtra = ["prvReadCustomAPI", "prvReadUser", "prvAppendToUser"];

var roles = new (string Name, string Description, string[] Privileges)[]
{
    ("Rule Studio Author", "Builds, tests, publishes and rolls back Rule Studio rules; manages functions and lookups.", Privileges(authorVerbs).Concat(authorExtra).ToArray()),
    ("Rule Studio Viewer", "Read-only access to Rule Studio rules, versions, releases, timeline and execution logs.", Privileges(viewerVerbs).ToArray()),
};

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.Ordinal) { ["DataverseUrl"] = environment, ["DataverseCredentialType"] = "browser" }).Build());
services.AddLogging();
services.AddDataverseWithOrganizationServices(options =>
{
    options.DataverseUrl = environment;
    options.CredentialType = DataverseCredentialType.InteractiveBrowserCredential;
});
using var provider = services.BuildServiceProvider();
var service = provider.GetRequiredService<IOrganizationService>();
service.Execute(new WhoAmIRequest());

// The app: the one model-driven app in the RuleStudio solution (DeployApp creates it).
var appComponents = new QueryExpression("solutioncomponent") { ColumnSet = new ColumnSet("objectid") };
var solutionLink = appComponents.AddLink("solution", "solutionid", "solutionid");
solutionLink.LinkCriteria.AddCondition("uniquename", ConditionOperator.Equal, solution);
appComponents.Criteria.AddCondition("componenttype", ConditionOperator.Equal, appModuleComponent);
var appIds = service.RetrieveMultiple(appComponents).Entities.Select(c => c.GetAttributeValue<Guid>("objectid")).ToArray();
if (appIds.Length != 1)
    throw new InvalidOperationException($"Expected one app in solution {solution}, found {appIds.Length}; run DeployApp first. Stopped.");
var appId = appIds[0];

var rootBu = service.RetrieveMultiple(new QueryExpression("businessunit")
{
    ColumnSet = new ColumnSet("name"),
    Criteria = { Conditions = { new ConditionExpression("parentbusinessunitid", ConditionOperator.Null) } },
}).Entities.Single();
Console.WriteLine($"Connected to {environment} · root business unit {rootBu.GetAttributeValue<string>("name")}");

var privilegeNames = roles.SelectMany(r => r.Privileges).Distinct(StringComparer.Ordinal).ToArray();
var privQuery = new QueryExpression("privilege") { ColumnSet = new ColumnSet("name") };
privQuery.Criteria.AddCondition("name", ConditionOperator.In, privilegeNames.Cast<object>().ToArray());
var privilegeIds = service.RetrieveMultiple(privQuery).Entities.ToDictionary(p => p.GetAttributeValue<string>("name"), p => p.Id, StringComparer.OrdinalIgnoreCase);
var missingPrivileges = privilegeNames.Where(n => !privilegeIds.ContainsKey(n)).ToArray();
if (missingPrivileges.Any(n => !authorExtra.Contains(n, StringComparer.Ordinal)))
    throw new InvalidOperationException("Table privileges not found (are the tables published?): " + string.Join(", ", missingPrivileges));

var results = new List<object>();
var roleIds = new List<Guid>();
foreach (var (name, description, privileges) in roles)
{
    var existing = service.RetrieveMultiple(new QueryExpression("role")
    {
        ColumnSet = new ColumnSet("roleid"),
        Criteria =
        {
            Conditions =
            {
                new ConditionExpression("name", ConditionOperator.Equal, name),
                new ConditionExpression("businessunitid", ConditionOperator.Equal, rootBu.Id),
            },
        },
    }).Entities.SingleOrDefault();
    Guid roleId;
    if (existing == null)
    {
        var create = new CreateRequest { Target = new Entity("role") { ["name"] = name, ["description"] = description, ["businessunitid"] = rootBu.ToEntityReference() } };
        create.Parameters["SolutionUniqueName"] = solution;
        roleId = ((CreateResponse)service.Execute(create)).id;
        Console.WriteLine($"  created  role {name}");
    }
    else
    {
        roleId = existing.Id;
        service.Execute(new AddSolutionComponentRequest { ComponentType = 20, ComponentId = roleId, SolutionUniqueName = solution });
        Console.WriteLine($"  exists   role {name}");
    }

    // Organization-owned tables only support organization (Global) depth.
    var granted = privileges.Where(privilegeIds.ContainsKey).ToArray();
    service.Execute(new AddPrivilegesRoleRequest
    {
        RoleId = roleId,
        Privileges = granted.Select(p => new RolePrivilege((int)PrivilegeDepth.Global, privilegeIds[p])).ToArray(),
    });
    Console.WriteLine($"  granted  {granted.Length} privileges to {name}");
    roleIds.Add(roleId);
    results.Add(new { name, roleId, privileges = granted });
}

// Link roles to the app so their members can open it.
var linkQuery = new QueryExpression("appmoduleroles") { ColumnSet = new ColumnSet("roleid") };
linkQuery.Criteria.AddCondition("appmoduleid", ConditionOperator.Equal, appId);
var linked = service.RetrieveMultiple(linkQuery).Entities.Select(e => e.GetAttributeValue<EntityReference>("roleid")?.Id ?? Guid.Empty).ToHashSet();
var toLink = roleIds.Where(r => !linked.Contains(r)).ToArray();
if (toLink.Length > 0)
{
    service.Associate("appmodule", appId, new Relationship("appmoduleroles_association"), new EntityReferenceCollection(toLink.Select(r => new EntityReference("role", r)).ToList()));
    service.Execute(new PublishXmlRequest { ParameterXml = $"<importexportxml><appmodules><appmodule>{appId}</appmodule></appmodules></importexportxml>" });
}

Console.WriteLine($"  linked   {toLink.Length} role(s) to the Rule Studio app ({roleIds.Count - toLink.Length} already linked)");

// Verify by read-back.
foreach (var roleId in roleIds)
{
    var rp = new QueryExpression("roleprivileges") { ColumnSet = new ColumnSet("privilegeid") };
    rp.Criteria.AddCondition("roleid", ConditionOperator.Equal, roleId);
    var count = service.RetrieveMultiple(rp).Entities.Count;
    if (count == 0)
        throw new InvalidOperationException($"Role {roleId} has no privileges after grant.");
}

File.WriteAllText("DeploySecurityRoles/result.json", JsonSerializer.Serialize(new { environment, solution, appId, roles = results, missingOptionalPrivileges = missingPrivileges }, new JsonSerializerOptions { WriteIndented = true }));
Console.WriteLine($"SUCCESS: {roleIds.Count} roles created/updated, privileges granted and app access linked.");
if (missingPrivileges.Length > 0)
{
    Console.WriteLine("NOTE optional privileges not found: " + string.Join(", ", missingPrivileges));
}

IEnumerable<string> Privileges(string[] verbs) => tables.SelectMany(t => verbs.Select(v => $"prv{v}{t}"));

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
        using var json = JsonDocument.Parse(File.ReadAllText(file));
        if (json.RootElement.TryGetProperty("DataverseUrl", out var url) && url.GetString() is { Length: > 0 } value && !value.Contains("yourorg", StringComparison.OrdinalIgnoreCase))
            return value;
    }

    throw new InvalidOperationException("Set DataverseUrl (environment variable or appsettings.local.json) to your Dataverse environment URL.");
}
