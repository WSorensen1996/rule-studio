// Creates the Rule Studio data model (10 tables, 5 global choices, columns, lookups, alternate key)
// in the RuleStudio solution. Idempotent: anything that exists is skipped.
// Needs the RuleStudio solution with a publisher whose prefix is "rs" and option value prefix 12625.
// Run from the RuleStudioApp folder: dotnet run --project DeployDataModel
using System.ServiceModel;
using System.Text.Json;
using DataverseConnection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

var environment = ResolveDataverseUrl();
const string solution = "RuleStudio";
// The plug-ins and pages are compiled against choice values 126250000, 126250001, …
const int expectedOptionPrefix = 12625;

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["DataverseUrl"] = environment, ["DataverseCredentialType"] = "browser" }).Build());
services.AddLogging();
services.AddDataverseWithOrganizationServices(options =>
{
    options.DataverseUrl = environment;
    options.CredentialType = DataverseCredentialType.InteractiveBrowserCredential;
});
using var provider = services.BuildServiceProvider();
var service = provider.GetRequiredService<IOrganizationService>();
service.Execute(new WhoAmIRequest());

// Publisher prefix and option value prefix come from the RuleStudio solution's publisher.
var sol = service.RetrieveMultiple(new QueryExpression("solution")
{
    ColumnSet = new ColumnSet("publisherid"),
    Criteria = { Conditions = { new ConditionExpression("uniquename", ConditionOperator.Equal, solution) } },
}).Entities.SingleOrDefault() ?? throw new InvalidOperationException($"Solution {solution} not found; create it with an 'rs' publisher first. Stopped.");
var publisher = service.Retrieve("publisher", sol.GetAttributeValue<EntityReference>("publisherid").Id, new ColumnSet("customizationprefix", "customizationoptionvalueprefix"));
if (publisher.GetAttributeValue<string>("customizationprefix") != "rs")
    throw new InvalidOperationException("RuleStudio publisher prefix is not 'rs'; stopped.");

var optionPrefix = publisher.GetAttributeValue<int>("customizationoptionvalueprefix");
if (optionPrefix != expectedOptionPrefix)
    throw new InvalidOperationException($"RuleStudio publisher option value prefix is {optionPrefix}, expected {expectedOptionPrefix}; stopped.");
var lcid = service.RetrieveMultiple(new QueryExpression("organization") { ColumnSet = new ColumnSet("languagecode") }).Entities.Single().GetAttributeValue<int>("languagecode");
Console.WriteLine($"Connected to {environment} · publisher prefix rs · option prefix {optionPrefix} · base language {lcid}");

Label L(string text) => new(text, lcid);
OptionMetadata[] Options(string[] labels) => labels.Select((x, i) => new OptionMetadata(L(x), (optionPrefix * 10000) + i)).ToArray();
var required = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.ApplicationRequired);
var optional = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None);

// ── Definitions ──────────────────────────────────────────────────────────
var globalChoices = new (string Name, string Display, string[] Values)[]
{
    ("rs_event", "Event", ["Create", "Update", "Delete"]),
    ("rs_stage", "Stage", ["Before save", "After save"]),
    ("rs_valuesourcetype", "Value source", ["Fixed", "Field path", "Expression", "Lookup", "Function"]),
    ("rs_loglevel", "Log level", ["Off", "Errors only", "Summary", "Full trace"]),
    ("rs_messagelevel", "Message level", ["Info", "Warning", "Error"]),
};

var tables = new TableDef[]
{
    new("rs_Rule", "Rule", "Rules", 200, "Stable rule identity and live/draft pointers. The trigger is versioned on Rule Version.",
    [
        new TextCol("rs_RuleKey", "Rule key", 100, Required: true),
        new MemoCol("rs_Description", "Description", 4000),
        new YesNoCol("rs_Enabled", "Enabled", true),
        new LookupCol("rs_PublishedVersionId", "Published version", "rs_ruleversion", Cascade.RemoveLink),
        new LookupCol("rs_DraftVersionId", "Draft version", "rs_ruleversion", Cascade.RemoveLink),
        new TextCol("rs_LastDeployedHash", "Last deployed hash", 128),
    ]),
    new("rs_RuleVersion", "Rule Version", "Rule Versions", 200, "Draft or immutable published version of a rule with its compiled snapshot.",
    [
        new LookupCol("rs_RuleId", "Rule", "rs_rule", Cascade.Delete, Required: true),
        new WholeCol("rs_VersionNumber", "Version number", Required: true),
        new LocalChoiceCol("rs_Status", "Status", ["Draft", "Published", "Superseded", "Discarded"]),
        new TextCol("rs_TargetTable", "Target table", 100, Required: true),
        new GlobalChoiceCol("rs_Event", "Event", "rs_event", Required: true),
        new GlobalChoiceCol("rs_Stage", "Stage", "rs_stage", Required: true),
        new TextCol("rs_FilteredColumns", "Triggering columns", 1000),
        new WholeCol("rs_Priority", "Priority"),
        new YesNoCol("rs_StopProcessing", "Stop processing after match", false),
        new LocalChoiceCol("rs_FailurePolicy", "On failure", ["Block", "Log and continue"]),
        new WholeCol("rs_RetryCount", "Retry count"),
        new WholeCol("rs_RetryDelayMs", "Retry delay (ms)"),
        new LocalChoiceCol("rs_RunAs", "Run as", ["Calling user", "System"]),
        new GlobalChoiceCol("rs_LogLevel", "Execution log level", "rs_loglevel"),
        new MemoCol("rs_CompiledSnapshot", "Compiled snapshot", 1048576),
        new TextCol("rs_SnapshotHash", "Snapshot hash", 128),
        new TextCol("rs_BaseHash", "Base hash", 128),
        new LocalChoiceCol("rs_Source", "Source", ["Authored", "Imported"]),
        new LocalChoiceCol("rs_CostLevel", "Cost level", ["Low", "Medium", "High"]),
        new LookupCol("rs_ReleaseId", "Release", "rs_release", Cascade.RemoveLink),
        new DateCol("rs_PublishedOn", "Published on"),
        new LookupCol("rs_PublishedBy", "Published by", "systemuser", Cascade.RemoveLink),
        new MemoCol("rs_ChangeNote", "Change note", 2000),
        new LookupCol("rs_DerivedFromVersionId", "Derived from version", "rs_ruleversion", Cascade.RemoveLink),
    ]),
    new("rs_ConditionGroup", "Condition Group", "Condition Groups", 100, "Nested AND/OR group of conditions.",
    [
        new LookupCol("rs_RuleVersionId", "Rule version", "rs_ruleversion", Cascade.Delete, Required: true),
        new LookupCol("rs_ParentGroupId", "Parent group", "rs_conditiongroup", Cascade.Delete),
        new LocalChoiceCol("rs_LogicalOperator", "Logical operator", ["AND", "OR"]),
        new WholeCol("rs_Order", "Order"),
        new LookupCol("rs_AggregateScopeConditionId", "Aggregate scope condition", "rs_condition", Cascade.RemoveLink),
    ]),
    new("rs_Condition", "Condition", "Conditions", 200, "Field path → operator → value predicate.",
    [
        new LookupCol("rs_ConditionGroupId", "Condition group", "rs_conditiongroup", Cascade.Delete, Required: true),
        new WholeCol("rs_Order", "Order"),
        new MemoCol("rs_FieldPath", "Field path", 4000),
        new LocalChoiceCol("rs_Aggregate", "Child operator", ["None", "Any", "All", "None-of", "Count", "Sum", "Min", "Max", "Average"]),
        new LocalChoiceCol("rs_Operator", "Comparison", ["Equals", "Not equals", "Greater than", "Greater or equal", "Less than", "Less or equal", "Contains", "Starts with", "Is empty", "Is not empty", "Changed", "In list", "Between"]),
        new GlobalChoiceCol("rs_ValueSourceType", "Value source", "rs_valuesourcetype"),
        new TextCol("rs_Value", "Value", 4000),
        new MemoCol("rs_ValuePath", "Value path", 4000),
        new LookupCol("rs_LookupDefinitionId", "Lookup definition", "rs_lookupdefinition", Cascade.Restrict),
        new LookupCol("rs_CustomFunctionId", "Custom function", "rs_customfunction", Cascade.Restrict),
    ]),
    new("rs_Action", "Action", "Actions", 200, "Ordered THEN/ELSE action of a rule version.",
    [
        new LookupCol("rs_RuleVersionId", "Rule version", "rs_ruleversion", Cascade.Delete, Required: true),
        new LocalChoiceCol("rs_Branch", "Branch", ["Then", "Else"]),
        new WholeCol("rs_Order", "Order"),
        new LocalChoiceCol("rs_ActionType", "Action type", ["Set field", "Block save", "Call function", "Log message"]),
        new TextCol("rs_TargetField", "Target field", 100),
        new GlobalChoiceCol("rs_ValueSourceType", "Value source", "rs_valuesourcetype"),
        new TextCol("rs_Value", "Value", 4000),
        new MemoCol("rs_ValuePath", "Value path", 4000),
        new LookupCol("rs_LookupDefinitionId", "Lookup definition", "rs_lookupdefinition", Cascade.Restrict),
        new LookupCol("rs_CustomFunctionId", "Custom function", "rs_customfunction", Cascade.Restrict),
        new MemoCol("rs_Message", "Message", 2000),
        new GlobalChoiceCol("rs_MessageLevel", "Message level", "rs_messagelevel"),
    ]),
    new("rs_ActionInputMapping", "Action Input Mapping", "Action Input Mappings", 100, "Typed mapping of a value to a custom function input parameter.",
    [
        new LookupCol("rs_ActionId", "Action", "rs_action", Cascade.Delete),
        new LookupCol("rs_ConditionId", "Condition", "rs_condition", Cascade.Delete),
        new TextCol("rs_ParameterName", "Parameter name", 100, Required: true),
        new GlobalChoiceCol("rs_ValueSourceType", "Value source", "rs_valuesourcetype"),
        new TextCol("rs_Value", "Value", 4000),
        new MemoCol("rs_ValuePath", "Value path", 4000),
    ]),
    new("rs_CustomFunction", "Custom Function", "Custom Functions", 100, "Registry of developer-built Custom APIs usable in rules.",
    [
        new MemoCol("rs_Description", "Description", 2000),
        new TextCol("rs_CustomApiName", "Custom API name", 128, Required: true),
        new MemoCol("rs_InputSchema", "Input schema", 10000),
        new LocalChoiceCol("rs_OutputType", "Output type", ["Text", "Number", "Decimal", "Money", "Boolean", "Date", "Lookup"]),
        new YesNoCol("rs_HasSideEffects", "Has side effects", false),
        new YesNoCol("rs_SafeToRetry", "Safe to retry", true),
        new WholeCol("rs_TimeoutMs", "Timeout (ms)"),
        new YesNoCol("rs_Enabled", "Enabled", true),
    ]),
    new("rs_LookupDefinition", "Lookup Definition", "Lookup Definitions", 100, "Reusable value source reading from any Dataverse table.",
    [
        new MemoCol("rs_Description", "Description", 2000),
        new TextCol("rs_LookupTable", "Lookup table", 100),
        new MemoCol("rs_MatchCriteria", "Match criteria", 10000),
        new TextCol("rs_ReturnColumn", "Return column", 100),
        new LocalChoiceCol("rs_NoMatchPolicy", "If no rows match", ["Block", "Use default", "Skip"]),
        new TextCol("rs_DefaultValue", "Default value", 1000),
        new LocalChoiceCol("rs_MultiMatchPolicy", "If multiple rows match", ["Most specific", "First by sort", "Error"]),
        new TextCol("rs_SortColumn", "Sort column", 100),
    ]),
    new("rs_Release", "Release", "Releases", 200, "Atomic publication of one or more rule versions; unit of rollback.",
    [
        new MemoCol("rs_Description", "Description", 2000),
        new LocalChoiceCol("rs_Status", "Status", ["Live", "Rolled back", "Failed"]),
        new DateCol("rs_PublishedOn", "Published on"),
        new LookupCol("rs_PublishedBy", "Published by", "systemuser", Cascade.RemoveLink),
        new LookupCol("rs_RolledBackReleaseId", "Rolled back release", "rs_release", Cascade.RemoveLink),
        new MemoCol("rs_ValidationResult", "Validation result", 100000),
    ]),
    new("rs_ExecutionLog", "Execution Log", "Execution Logs", 200, "Per-rule execution outcome, duration and trace.",
    [
        new LookupCol("rs_RuleVersionId", "Rule version", "rs_ruleversion", Cascade.Restrict),
        new LookupCol("rs_RuleId", "Rule", "rs_rule", Cascade.RemoveLink),
        new TextCol("rs_TargetTable", "Target table", 100),
        new TextCol("rs_RecordId", "Record ID", 100),
        new GlobalChoiceCol("rs_Event", "Event", "rs_event"),
        new GlobalChoiceCol("rs_Stage", "Stage", "rs_stage"),
        new LocalChoiceCol("rs_Outcome", "Outcome", ["Matched", "Not matched", "Skipped", "Failed", "Blocked"]),
        new MemoCol("rs_Trace", "Trace", 1048576),
        new WholeCol("rs_DurationMs", "Duration (ms)"),
        new WholeCol("rs_Attempts", "Attempts"),
        new MemoCol("rs_ErrorMessage", "Error message", 4000),
        new TextCol("rs_CorrelationId", "Correlation ID", 100),
        new YesNoCol("rs_IsTest", "Is test", false),
        new YesNoCol("rs_AdminFlag", "Admin flag", false),
        new GlobalChoiceCol("rs_LogLevel", "Log level", "rs_loglevel"),
    ]),
};

var log = new List<string>();
void Note(string line)
{
    Console.WriteLine(line);
    log.Add(line);
}

bool Exists(Action probe)
{
    try
    {
        probe();
        return true;
    }
    catch (FaultException<OrganizationServiceFault>)
    {
        return false;
    }
}

// ── 1. Global choices ────────────────────────────────────────────────────
foreach (var c in globalChoices)
{
    if (Exists(() => service.Execute(new RetrieveOptionSetRequest { Name = c.Name })))
    {
        Note($"  exists   choice {c.Name}");
        continue;
    }

    var os = new OptionSetMetadata(new OptionMetadataCollection(Options(c.Values))) { Name = c.Name, DisplayName = L(c.Display), IsGlobal = true, OptionSetType = OptionSetType.Picklist };
    service.Execute(new CreateOptionSetRequest { OptionSet = os, SolutionUniqueName = solution });
    Note($"  created  choice {c.Name}");
}

// ── 2. Tables ────────────────────────────────────────────────────────────
foreach (var t in tables)
{
    if (Exists(() => service.Execute(new RetrieveEntityRequest { LogicalName = t.Logical, EntityFilters = EntityFilters.Entity })))
    {
        Note($"  exists   table {t.Logical}");
        continue;
    }

    service.Execute(new CreateEntityRequest
    {
        Entity = new EntityMetadata
        {
            SchemaName = t.Schema,
            DisplayName = L(t.Display),
            DisplayCollectionName = L(t.Plural),
            Description = L(t.Description),
            OwnershipType = OwnershipTypes.OrganizationOwned,
            IsActivity = false,
        },
        PrimaryAttribute = new StringAttributeMetadata
        {
            SchemaName = "rs_Name",
            RequiredLevel = required,
            MaxLength = t.NameLength,
            FormatName = StringFormatName.Text,
            DisplayName = L("Name"),
            Description = L("Primary name."),
        },
        HasActivities = false,
        HasNotes = false,
        SolutionUniqueName = solution,
    });
    Note($"  created  table {t.Logical}");
}

// ── 3. Columns (non-lookup) ──────────────────────────────────────────────
HashSet<string> AttributesOf(string table)
{
    var meta = (RetrieveEntityResponse)service.Execute(new RetrieveEntityRequest { LogicalName = table, EntityFilters = EntityFilters.Attributes, RetrieveAsIfPublished = true });
    return meta.EntityMetadata.Attributes.Select(a => a.LogicalName).ToHashSet(StringComparer.Ordinal);
}

foreach (var t in tables)
{
    var existing = AttributesOf(t.Logical);
    foreach (var col in t.Columns.Where(c => c is not LookupCol))
    {
        if (existing.Contains(col.Logical))
        {
            Note($"  exists   {t.Logical}.{col.Logical}");
            continue;
        }

        AttributeMetadata attr = col switch
        {
            TextCol x => new StringAttributeMetadata { MaxLength = x.Max, FormatName = StringFormatName.Text },
            MemoCol x => new MemoAttributeMetadata { MaxLength = x.Max, Format = StringFormat.TextArea },
            WholeCol => new IntegerAttributeMetadata { Format = IntegerFormat.None, MinValue = 0, MaxValue = int.MaxValue },
            YesNoCol x => new BooleanAttributeMetadata(new BooleanOptionSetMetadata(new OptionMetadata(L("Yes"), 1), new OptionMetadata(L("No"), 0))) { DefaultValue = x.Default },
            DateCol => new DateTimeAttributeMetadata { Format = DateTimeFormat.DateAndTime, DateTimeBehavior = DateTimeBehavior.UserLocal },
            GlobalChoiceCol x => new PicklistAttributeMetadata { OptionSet = new OptionSetMetadata { IsGlobal = true, Name = x.OptionSet } },
            LocalChoiceCol x => new PicklistAttributeMetadata { OptionSet = new OptionSetMetadata(new OptionMetadataCollection(Options(x.Values))) { IsGlobal = false, OptionSetType = OptionSetType.Picklist } },
            _ => throw new InvalidOperationException(col.GetType().Name),
        };
        attr.SchemaName = col.Schema;
        attr.DisplayName = L(col.Display);
        attr.RequiredLevel = col.Required ? required : optional;
        service.Execute(new CreateAttributeRequest { EntityName = t.Logical, Attribute = attr, SolutionUniqueName = solution });
        Note($"  created  {t.Logical}.{col.Logical}");
    }
}

// ── 3b. Local choices: add values introduced after the column was created ──
foreach (var t in tables)
{
    foreach (var col in t.Columns.OfType<LocalChoiceCol>())
    {
        var attr = (PicklistAttributeMetadata)((RetrieveAttributeResponse)service.Execute(new RetrieveAttributeRequest { EntityLogicalName = t.Logical, LogicalName = col.Logical, RetrieveAsIfPublished = true })).AttributeMetadata;
        var values = attr.OptionSet.Options.Select(o => o.Value).ToHashSet();
        foreach (var option in Options(col.Values).Where(o => !values.Contains(o.Value)))
        {
            service.Execute(new InsertOptionValueRequest { EntityLogicalName = t.Logical, AttributeLogicalName = col.Logical, Value = option.Value, Label = option.Label, SolutionUniqueName = solution });
            Note($"  added    {t.Logical}.{col.Logical} option {option.Label.LocalizedLabels[0].Label} ({option.Value})");
        }
    }
}

// ── 4. Lookups (relationships) ───────────────────────────────────────────
var deviations = new List<string>();
CascadeConfiguration CascadeFor(Cascade c) => new()
{
    Assign = CascadeType.NoCascade,
    Share = CascadeType.NoCascade,
    Unshare = CascadeType.NoCascade,
    Reparent = CascadeType.NoCascade,
    Merge = CascadeType.NoCascade,
    RollupView = CascadeType.NoCascade,
    Delete = c switch
    {
        Cascade.Delete => CascadeType.Cascade,
        Cascade.Restrict => CascadeType.Restrict,
        _ => CascadeType.RemoveLink,
    },
};

foreach (var t in tables)
{
    var existing = AttributesOf(t.Logical);
    foreach (var col in t.Columns.OfType<LookupCol>())
    {
        if (existing.Contains(col.Logical))
        {
            Note($"  exists   {t.Logical}.{col.Logical} → {col.Target}");
            continue;
        }

        CreateOneToManyRequest Request(Cascade cascade) => new()
        {
            Lookup = new LookupAttributeMetadata { SchemaName = col.Schema, DisplayName = L(col.Display), RequiredLevel = col.Required ? required : optional },
            OneToManyRelationship = new OneToManyRelationshipMetadata
            {
                SchemaName = $"rs_{col.Target}_{t.Logical}_{col.Logical}",
                ReferencedEntity = col.Target,
                ReferencingEntity = t.Logical,
                CascadeConfiguration = CascadeFor(cascade),
                AssociatedMenuConfiguration = new AssociatedMenuConfiguration { Behavior = AssociatedMenuBehavior.UseCollectionName, Group = AssociatedMenuGroup.Details, Order = 10000 },
            },
            SolutionUniqueName = solution,
        };
        try
        {
            service.Execute(Request(col.Cascade));
            Note($"  created  {t.Logical}.{col.Logical} → {col.Target} ({col.Cascade})");
        }
        catch (FaultException<OrganizationServiceFault> ex) when (col.Cascade == Cascade.Delete)
        {
            // Dataverse limits cascade-delete paths (one parental relationship per table, no cycles).
            service.Execute(Request(Cascade.RemoveLink));
            var d = $"{t.Logical}.{col.Logical} → {col.Target}: cascade delete refused ({ex.Detail.Message}); created with RemoveLink instead.";
            deviations.Add(d);
            Note($"  created  {d}");
        }
    }
}

// ── 5. Alternate key on Rule.rs_rulekey ─────────────────────────────────
if (Exists(() => service.Execute(new RetrieveEntityKeyRequest { EntityLogicalName = "rs_rule", LogicalName = "rs_rulekey_key" })))
{
    Note("  exists   key rs_rule.rs_rulekey");
}
else
{
    service.Execute(new CreateEntityKeyRequest
    {
        EntityName = "rs_rule",
        EntityKey = new EntityKeyMetadata { SchemaName = "rs_RuleKey_Key", DisplayName = L("Rule key"), KeyAttributes = ["rs_rulekey"] },
        SolutionUniqueName = solution,
    });
    Note("  created  key rs_rule.rs_rulekey (index builds asynchronously)");
}

// ── 6. Publish & verify ─────────────────────────────────────────────────
var entitiesXml = string.Concat(tables.Select(t => $"<entity>{t.Logical}</entity>"));
var optionSetsXml = string.Concat(globalChoices.Select(c => $"<optionset>{c.Name}</optionset>"));
service.Execute(new PublishXmlRequest { ParameterXml = $"<importexportxml><entities>{entitiesXml}</entities><optionsets>{optionSetsXml}</optionsets></importexportxml>" });
Note("Published all Rule Studio tables and choices.");

var missing = new List<string>();
foreach (var t in tables)
{
    var attrs = AttributesOf(t.Logical);
    missing.AddRange(t.Columns.Select(c => c.Logical).Append("rs_name").Where(c => !attrs.Contains(c)).Select(c => $"{t.Logical}.{c}"));
    foreach (var col in t.Columns.OfType<LocalChoiceCol>())
    {
        var options = ((PicklistAttributeMetadata)((RetrieveAttributeResponse)service.Execute(new RetrieveAttributeRequest { EntityLogicalName = t.Logical, LogicalName = col.Logical })).AttributeMetadata).OptionSet.Options.Select(o => o.Value).ToHashSet();
        missing.AddRange(Options(col.Values).Where(o => !options.Contains(o.Value)).Select(o => $"{t.Logical}.{col.Logical} option {o.Value}"));
    }
}

File.WriteAllText("DeployDataModel/result.json", JsonSerializer.Serialize(new { environment, solution, optionPrefix, lcid, tables = tables.Select(t => t.Logical), columns = tables.Sum(t => t.Columns.Length + 1), deviations, missing, log }, new JsonSerializerOptions { WriteIndented = true }));
if (missing.Count > 0)
    throw new InvalidOperationException("Verification failed, missing: " + string.Join(", ", missing));

Console.WriteLine($"SUCCESS: {tables.Length} tables, {globalChoices.Length} global choices and {tables.Sum(t => t.Columns.Length + 1)} columns verified. Deviations: {deviations.Count}");
foreach (var d in deviations)
{
    Console.WriteLine("  DEVIATION " + d);
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
        using var json = JsonDocument.Parse(File.ReadAllText(file));
        if (json.RootElement.TryGetProperty("DataverseUrl", out var url) && url.GetString() is { Length: > 0 } value && !value.Contains("yourorg", StringComparison.OrdinalIgnoreCase))
            return value;
    }

    throw new InvalidOperationException("Set DataverseUrl (environment variable or appsettings.local.json) to your Dataverse environment URL.");
}

internal enum Cascade
{
    RemoveLink,
    Delete,
    Restrict,
}

internal abstract record ColDef(string Schema, string Display, bool Required = false)
{
    public string Logical => Schema.ToLowerInvariant();
}

internal sealed record TextCol(string Schema, string Display, int Max, bool Required = false) : ColDef(Schema, Display, Required);

internal sealed record MemoCol(string Schema, string Display, int Max) : ColDef(Schema, Display);

internal sealed record WholeCol(string Schema, string Display, bool Required = false) : ColDef(Schema, Display, Required);

internal sealed record YesNoCol(string Schema, string Display, bool Default) : ColDef(Schema, Display);

internal sealed record DateCol(string Schema, string Display) : ColDef(Schema, Display);

internal sealed record GlobalChoiceCol(string Schema, string Display, string OptionSet, bool Required = false) : ColDef(Schema, Display, Required);

internal sealed record LocalChoiceCol(string Schema, string Display, string[] Values) : ColDef(Schema, Display);

internal sealed record LookupCol(string Schema, string Display, string Target, Cascade Cascade, bool Required = false) : ColDef(Schema, Display, Required);

internal sealed record TableDef(string Schema, string Display, string Plural, int NameLength, string Description, ColDef[] Columns)
{
    public string Logical => Schema.ToLowerInvariant();
}
