// Uploads the Rule Studio pages and icons as web resources in the RuleStudio solution and adds the
// "Rule Studio" area to the model-driven app's sitemap. Uses the app in the RuleStudio solution, or
// creates one (rs_RuleStudio) with its sitemap when the solution has none. Existing navigation is preserved.
// Build the pages first: `npm ci && npm run build` in src/Dataverse/WebResources.
// Run from the RuleStudioApp folder: dotnet run --project DeployApp
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using DataverseConnection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

var environment = ResolveDataverseUrl();
const string solution = "RuleStudio";
const string appUniqueName = "rs_RuleStudio";
const string appName = "Rule Studio";
const string prefix = "rs_RuleStudio/html/";
const string sourceDir = "src/Dataverse/WebResources/dist/rs_RuleStudio/html";
const string iconPrefix = "rs_RuleStudio/icons/";
const string iconDir = "src/Dataverse/WebResources/dist/rs_RuleStudio/icons";
const string areaId = "rs_rulestudio_area";
const int appModuleComponent = 80;
const int sitemapComponent = 62;
const int webResourceComponent = 61;

// Sitemap groups → (file, title). Record pages and dialogs are uploaded but not placed in navigation.
var navigation = new (string Id, string Title, (string File, string Title)[] Pages)[]
{
    ("rs_grp_workspace", "Workspace", [("overview", "Overview"), ("rule-library", "Rule library"), ("execution-timeline", "Execution timeline"), ("test-trace", "Test & trace")]),
    ("rs_grp_buildingblocks", "Building blocks", [("custom-functions", "Custom functions"), ("lookup-definitions", "Lookup definitions")]),
    ("rs_grp_operations", "Operations", [("releases", "Releases & versions"), ("execution-logs", "Execution logs"), ("environments", "Environments")]),
    ("rs_grp_reference", "Reference", [("guide", "Documentation")]),
};
string[] notInNavigation = ["rule-editor", "release-detail", "log-detail", "register-function", "new-lookup-definition", "publish-release", "rollback-release", "restore-version"];
var all = navigation.SelectMany(g => g.Pages.Select(p => p.File)).Concat(notInNavigation).ToArray();
foreach (var file in all)
    if (!File.Exists(Path.Combine(sourceDir, file + ".html")))
        throw new FileNotFoundException("Run `npm ci && npm run build` in src/Dataverse/WebResources first.", file + ".html");

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
var solutionQuery = new QueryExpression("solution") { ColumnSet = new ColumnSet("solutionid") };
solutionQuery.Criteria.AddCondition("uniquename", ConditionOperator.Equal, solution);
var solutionId = service.RetrieveMultiple(solutionQuery).Entities.SingleOrDefault()?.Id
    ?? throw new InvalidOperationException($"Solution {solution} not found; run DeployDataModel first. Stopped.");
Console.WriteLine($"Connected to {environment} · solution {solution}");

// 1. Upsert web resources (only the names listed above are touched): pages (type 1) and SVG icons (type 11).
var icons = Directory.GetFiles(iconDir, "*.svg").Select(f => Path.GetFileNameWithoutExtension(f)).OrderBy(f => f, StringComparer.Ordinal).ToArray();
var resources = all.Select(f => (Name: prefix + f + ".html", Path: Path.Combine(sourceDir, f + ".html"), Type: 1, Label: f))
    .Concat(icons.Select(f => (Name: iconPrefix + f + ".svg", Path: Path.Combine(iconDir, f + ".svg"), Type: 11, Label: f + " icon")));
var ids = new Dictionary<string, Guid>(StringComparer.Ordinal);
foreach (var (name, path, type, file) in resources)
{
    var content = File.ReadAllText(path);
    var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
    var displayName = $"Rule Studio · {file}";
    var query = new QueryExpression("webresource") { ColumnSet = new ColumnSet("content", "displayname") };
    query.Criteria.AddCondition("name", ConditionOperator.Equal, name);
    var existing = service.RetrieveMultiple(query).Entities.SingleOrDefault();
    Guid id;
    if (existing == null)
    {
        var create = new CreateRequest { Target = new Entity("webresource") { ["name"] = name, ["displayname"] = displayName, ["webresourcetype"] = new OptionSetValue(type), ["content"] = base64, ["description"] = "Rule Studio page." } };
        create.Parameters["SolutionUniqueName"] = solution;
        id = ((CreateResponse)service.Execute(create)).id;
        Console.WriteLine($"  created  {name}");
    }
    else
    {
        id = existing.Id;
        if (existing.GetAttributeValue<string>("content") != base64 || existing.GetAttributeValue<string>("displayname") != displayName)
        {
            service.Update(new Entity("webresource", id) { ["content"] = base64, ["displayname"] = displayName, ["description"] = "Rule Studio page." });
            Console.WriteLine($"  updated  {name}");
        }
        else
        {
            Console.WriteLine($"  same     {name}");
        }

        service.Execute(new AddSolutionComponentRequest { ComponentType = webResourceComponent, ComponentId = id, SolutionUniqueName = solution });
    }

    ids[name] = id;
}

// The "Rule Studio" sitemap area with the navigation groups above.
string IconUrl(string file) => "$webresource:" + iconPrefix + (icons.Contains(file) ? file : "rule-studio") + ".svg";
XElement BuildArea() => new("Area", new XAttribute("Id", areaId), new XAttribute("ShowGroups", "true"),
    new XAttribute("Icon", IconUrl("rule-studio")), new XAttribute("VectorIcon", IconUrl("rule-studio")),
    new XElement("Titles", new XElement("Title", new XAttribute("LCID", "1033"), new XAttribute("Title", "Rule Studio"))),
    navigation.Select(g => new XElement("Group", new XAttribute("Id", g.Id), new XAttribute("IsProfile", "false"),
        new XElement("Titles", new XElement("Title", new XAttribute("LCID", "1033"), new XAttribute("Title", g.Title))),
        g.Pages.Select(p => new XElement("SubArea",
            new XAttribute("Id", "rs_sub_" + p.File.Replace('-', '_')),
            new XAttribute("Url", "$webresource:" + prefix + p.File + ".html"),
            new XAttribute("Icon", IconUrl(p.File)), new XAttribute("VectorIcon", IconUrl(p.File)),
            new XAttribute("Client", "Web"), new XAttribute("AvailableOffline", "false"), new XAttribute("PassParams", "false"),
            new XElement("Titles", new XElement("Title", new XAttribute("LCID", "1033"), new XAttribute("Title", p.Title))))))));

// 2. The app: the one model-driven app in the solution, else a new one. An app without a sitemap
//    gets one with the Rule Studio area (Dataverse refuses an empty sitemap).
var appComponents = new QueryExpression("solutioncomponent") { ColumnSet = new ColumnSet("objectid") };
appComponents.Criteria.AddCondition("solutionid", ConditionOperator.Equal, solutionId);
appComponents.Criteria.AddCondition("componenttype", ConditionOperator.Equal, appModuleComponent);
var appIds = service.RetrieveMultiple(appComponents).Entities.Select(c => c.GetAttributeValue<Guid>("objectid")).ToArray();
if (appIds.Length > 1)
    throw new InvalidOperationException($"Solution {solution} contains {appIds.Length} apps; expected one. Stopped.");
Guid appId;
if (appIds.Length == 1)
{
    appId = appIds[0];
}
else
{
    var createApp = new CreateRequest
    {
        Target = new Entity("appmodule")
        {
            ["name"] = appName,
            ["uniquename"] = appUniqueName,
            ["description"] = "Build, test, publish and monitor Rule Studio rules.",
            ["webresourceid"] = ids[iconPrefix + "rule-studio.svg"],
            ["clienttype"] = 4,
            ["formfactor"] = 1,
            ["navigationtype"] = new OptionSetValue(0),
        },
    };
    createApp.Parameters["SolutionUniqueName"] = solution;
    appId = ((CreateResponse)service.Execute(createApp)).id;
    Console.WriteLine($"  created  app {appUniqueName}");
}

// A new app stays unpublished until step 4, so it is read with RetrieveUnpublished; a sitemap that
// is not yet linked in the published app is found by its unique name.
var app = Read("appmodule", appId, "name", "uniquename", "appmoduleidunique");
var sitemapQuery = new QueryExpression("appmodulecomponent") { ColumnSet = new ColumnSet("objectid") };
sitemapQuery.Criteria.AddCondition("appmoduleidunique", ConditionOperator.Equal, app.GetAttributeValue<Guid>("appmoduleidunique"));
sitemapQuery.Criteria.AddCondition("componenttype", ConditionOperator.Equal, sitemapComponent);
var sitemapByName = new QueryExpression("sitemap") { ColumnSet = new ColumnSet("sitemapid") };
sitemapByName.Criteria.AddCondition("sitemapnameunique", ConditionOperator.Equal, app.GetAttributeValue<string>("uniquename"));
var existingSitemapId = service.RetrieveMultiple(sitemapQuery).Entities.SingleOrDefault()?.GetAttributeValue<Guid>("objectid")
    ?? service.RetrieveMultiple(sitemapByName).Entities.SingleOrDefault()?.Id;
Guid sitemapId;
if (existingSitemapId is { } found)
{
    sitemapId = found;
}
else
{
    var createSitemap = new CreateRequest
    {
        Target = new Entity("sitemap")
        {
            ["sitemapname"] = appName,
            ["sitemapnameunique"] = app.GetAttributeValue<string>("uniquename"),
            ["isappaware"] = true,
            ["sitemapxml"] = new XElement("SiteMap", new XAttribute("IntroducedVersion", "7.0.0.0"), BuildArea()).ToString(SaveOptions.DisableFormatting),
        },
    };
    createSitemap.Parameters["SolutionUniqueName"] = solution;
    sitemapId = ((CreateResponse)service.Execute(createSitemap)).id;
    service.Execute(new AddAppComponentsRequest { AppId = appId, Components = [new EntityReference("sitemap", sitemapId)] });
    Console.WriteLine($"  created  sitemap {app.GetAttributeValue<string>("uniquename")}");
}

Console.WriteLine($"App {app.GetAttributeValue<string>("name")} ({app.GetAttributeValue<string>("uniquename")})");

// 3. Add (or replace) only the Rule Studio area in the sitemap; other areas stay as they are.
var sitemap = service.Retrieve("sitemap", sitemapId, new ColumnSet("sitemapxml"));
var original = sitemap.GetAttributeValue<string>("sitemapxml");
var xml = XDocument.Parse(original);
var root = xml.Root ?? throw new InvalidOperationException("Empty sitemap.");
Directory.CreateDirectory("DeployApp/backups");
File.WriteAllText($"DeployApp/backups/sitemap-before-{DateTime.UtcNow:yyyyMMddHHmmss}.xml", original);
root.Elements("Area").Where(a => (string?)a.Attribute("Id") == areaId).Remove();
root.AddFirst(BuildArea());
var current = service.Retrieve("sitemap", sitemapId, new ColumnSet("sitemapxml"));
if (current.GetAttributeValue<string>("sitemapxml") != original)
    throw new InvalidOperationException("Navigation changed during preparation; stopped to preserve it.");
service.Update(new Entity("sitemap", sitemapId) { ["sitemapxml"] = xml.ToString(SaveOptions.DisableFormatting) });
Console.WriteLine($"Sitemap: Rule Studio area with {navigation.Sum(g => g.Pages.Length)} pages added; existing areas preserved.");

// 4. Validate, publish, verify.
var validation = (ValidateAppResponse)service.Execute(new ValidateAppRequest { AppModuleId = appId });
File.WriteAllText("DeployApp/app-validation.json", JsonSerializer.Serialize(validation.AppValidationResponse, new JsonSerializerOptions { WriteIndented = true }));
if (!validation.AppValidationResponse.ValidationSuccess)
    throw new InvalidOperationException("App validation failed. See DeployApp/app-validation.json.");
var webresourcesXml = string.Concat(ids.Values.Select(i => $"<webresource>{i}</webresource>"));
service.Execute(new PublishXmlRequest { ParameterXml = $"<importexportxml><webresources>{webresourcesXml}</webresources><sitemaps><sitemap>{sitemapId}</sitemap></sitemaps><appmodules><appmodule>{appId}</appmodule></appmodules></importexportxml>" });

var verified = XDocument.Parse(service.Retrieve("sitemap", sitemapId, new ColumnSet("sitemapxml")).GetAttributeValue<string>("sitemapxml"));
var linked = verified.Descendants("SubArea").Count(x => ((string?)x.Attribute("Url") ?? string.Empty).StartsWith("$webresource:" + prefix, StringComparison.Ordinal));
var preserved = XDocument.Parse(original).Descendants("SubArea").Where(o => !((string?)o.Attribute("Id") ?? string.Empty).StartsWith("rs_sub_", StringComparison.Ordinal)).All(o => verified.Descendants("SubArea").Any(v => (string?)v.Attribute("Id") == (string?)o.Attribute("Id")));
if (linked < navigation.Sum(g => g.Pages.Length) || !preserved)
    throw new InvalidOperationException("Navigation verification failed.");

var appUrl = $"{environment.TrimEnd('/')}/main.aspx?appid={appId}&pagetype=webresource&webresourceName={Uri.EscapeDataString(prefix + "overview.html")}";
File.WriteAllText("DeployApp/result.json", JsonSerializer.Serialize(new { environment, solution, appId, sitemapId, webresources = ids.Keys, appUrl }, new JsonSerializerOptions { WriteIndented = true }));
Console.WriteLine($"SUCCESS: {ids.Count} web resources published ({icons.Length} icons), navigation verified ({linked} pages, existing entries preserved).");
Console.WriteLine(appUrl);

Entity Read(string table, Guid id, params string[] columns) =>
    ((RetrieveUnpublishedResponse)service.Execute(new RetrieveUnpublishedRequest { Target = new EntityReference(table, id), ColumnSet = new ColumnSet(columns) })).Entity;

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
