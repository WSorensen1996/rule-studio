// Deploys the Rule Studio engine plug-in (docs/engine.md): uploads the merged engine DLL as a
// sandboxed plug-in assembly in the RuleStudio solution, makes sure the RuleStudio.Engine.RulePlugin
// type exists, then calls rs_ReconcileSteps so the engine's steps match the published rules.
// Builds nothing: run `dotnet build src/Dataverse/RuleEngine -c Release` first, and deploy the
// Plugins assembly (XrmSync) before the first run, because rs_ReconcileSteps lives there.
// Run from the RuleStudioApp folder: dotnet run --project DeployEngine
using System.Reflection;
using DataverseConnection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

var environment = ResolveDataverseUrl();
const string solution = "RuleStudio";
const string dllPath = "src/Dataverse/RuleEngine/bin/Release/net462/Merged.RuleStudio.RuleEngine.dll";
const string typeName = "RuleStudio.Engine.RulePlugin";
const int pluginAssemblyComponent = 91;

if (!File.Exists(dllPath))
    throw new FileNotFoundException("Run `dotnet build src/Dataverse/RuleEngine -c Release` from the RuleStudioApp folder first.", dllPath);

var assemblyName = AssemblyName.GetAssemblyName(dllPath);
var token = assemblyName.GetPublicKeyToken();
if (token == null || token.Length == 0)
    throw new InvalidOperationException("The engine DLL is not signed; Dataverse only accepts strong-named plug-in assemblies.");
var publicKeyToken = Convert.ToHexString(token).ToLowerInvariant();
var name = assemblyName.Name ?? throw new InvalidOperationException("The engine DLL has no assembly name.");
var version = assemblyName.Version?.ToString() ?? "1.0.0.0";
var culture = string.IsNullOrEmpty(assemblyName.CultureName) ? "neutral" : assemblyName.CultureName;
Console.WriteLine($"Engine: {name} {version} ({publicKeyToken}), {new FileInfo(dllPath).Length / 1024} KB");

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
var who = (WhoAmIResponse)service.Execute(new WhoAmIRequest());
var solutions = new QueryExpression("solution") { ColumnSet = new ColumnSet("uniquename") };
solutions.Criteria.AddCondition("uniquename", ConditionOperator.Equal, solution);
if (service.RetrieveMultiple(solutions).Entities.Count != 1)
    throw new InvalidOperationException($"Solution {solution} not found; stopped.");
Console.WriteLine($"Connected to {environment} · solution {solution}");

// 1. Plug-in assembly: create, or update its content. Sandbox isolation, stored in the database.
var content = Convert.ToBase64String(File.ReadAllBytes(dllPath));
var assemblies = new QueryExpression("pluginassembly") { ColumnSet = new ColumnSet("name", "version", "isolationmode") };
assemblies.Criteria.AddCondition("name", ConditionOperator.Equal, name);
var existingAssembly = service.RetrieveMultiple(assemblies).Entities.SingleOrDefault();
Guid assemblyId;
if (existingAssembly == null)
{
    var create = new CreateRequest
    {
        Target = new Entity("pluginassembly")
        {
            ["name"] = name,
            ["content"] = content,
            ["version"] = version,
            ["culture"] = culture,
            ["publickeytoken"] = publicKeyToken,
            ["isolationmode"] = new OptionSetValue(2),
            ["sourcetype"] = new OptionSetValue(0),
            ["description"] = "Rule Studio engine: runs published rules. Not managed by XrmSync; steps are owned by rs_ReconcileSteps.",
        },
    };
    create.Parameters["SolutionUniqueName"] = solution;
    assemblyId = ((CreateResponse)service.Execute(create)).id;
    Console.WriteLine($"  created  plug-in assembly {name}");
}
else
{
    assemblyId = existingAssembly.Id;
    if (existingAssembly.GetAttributeValue<OptionSetValue>("isolationmode")?.Value != 2)
        throw new InvalidOperationException("The existing engine assembly is not sandboxed; stopped.");
    service.Update(new Entity("pluginassembly", assemblyId) { ["content"] = content, ["version"] = version });
    Console.WriteLine($"  updated  plug-in assembly {name} (was {existingAssembly.GetAttributeValue<string>("version")})");
}

// 2. Plug-in type.
var types = new QueryExpression("plugintype") { ColumnSet = new ColumnSet("typename") };
types.Criteria.AddCondition("typename", ConditionOperator.Equal, typeName);
types.Criteria.AddCondition("pluginassemblyid", ConditionOperator.Equal, assemblyId);
if (service.RetrieveMultiple(types).Entities.Count == 0)
{
    service.Create(new Entity("plugintype")
    {
        ["pluginassemblyid"] = new EntityReference("pluginassembly", assemblyId),
        ["typename"] = typeName,
        ["name"] = typeName,
        ["friendlyname"] = "Rule Studio engine",
        ["description"] = "Runs the published Rule Studio rules of the current table, event and stage.",
    });
    Console.WriteLine($"  created  plug-in type {typeName}");
}
else
{
    Console.WriteLine($"  same     plug-in type {typeName}");
}

// 3. The assembly (with its type) in the RuleStudio solution. Steps are not added: rs_ReconcileSteps
// rebuilds them in every environment from the published rules.
service.Execute(new AddSolutionComponentRequest { ComponentType = pluginAssemblyComponent, ComponentId = assemblyId, SolutionUniqueName = solution, AddRequiredComponents = false });
Console.WriteLine($"  added    assembly to solution {solution}");

// 4. Engine steps.
var reconcile = service.Execute(new OrganizationRequest("rs_ReconcileSteps"));
Console.WriteLine($"SUCCESS: {reconcile.Results["Summary"]}");

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
