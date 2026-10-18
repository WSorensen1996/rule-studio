using DataverseLogic.RuleStudio.Release;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using RuleStudio.SharedContext;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Steps;

/// <summary>
/// Makes the engine plug-in's sdkmessageprocessingstep rows match the published versions of the
/// enabled rules exactly (docs/engine.md). Only steps of the engine plug-in type
/// are touched. Runs as SYSTEM, so publishers need no System Customizer
/// role; it only ever brings the engine steps in line with what is published. Inside the publish
/// or rollback transaction, so a failure rolls the whole publish back.
/// </summary>
public class StepReconciler(IAdminDataverseAccessObjectService dao)
{
    // Type name of the engine plug-in (src/Dataverse/RuleEngine/RulePlugin.cs).
    public const string EngineTypeName = "RuleStudio.Engine.RulePlugin";

    private const string Step = "sdkmessageprocessingstep";
    private const string StepImage = "sdkmessageprocessingstepimage";
    private const string StepId = "sdkmessageprocessingstepid";
    private const string PreImageName = "PreImage";
    private const string FilteringAttributes = "filteringattributes";
    private const string AsyncAutoDelete = "asyncautodelete";
    private const string Name = "name";
    private const string Warning = "warning";
    private const int MaxRows = 5000;

    private readonly IAdminDataverseAccessObjectService dao = dao;

    // The engine plug-in type, or null when the engine assembly is not deployed yet.
    public Guid? EngineTypeId()
    {
        var query = new QueryExpression("plugintype") { ColumnSet = new ColumnSet("plugintypeid"), TopCount = 1 };
        query.Criteria.AddCondition("typename", ConditionOperator.Equal, EngineTypeName);
        return dao.RetrieveList<Entity>(query).FirstOrDefault()?.Id;
    }

    public ReconcileResult Reconcile()
    {
        var result = new ReconcileResult();
        var typeId = EngineTypeId();
        if (typeId == null)
        {
            result.Warnings.Add(new ValidationIssue
            {
                Severity = Warning,
                Code = ValidationCodes.EngineNotRegistered,
                Message = "The Rule Studio engine plug-in is not deployed in this environment, so published rules do not run yet. Deploy it with DeployEngine; that also creates the plug-in steps.",
            });
            return result;
        }

        result.EngineRegistered = true;
        var plan = StepPlanner.Plan(StepPlanner.Desired(PublishedTriggers()), ExistingSteps(typeId.Value));
        foreach (var step in plan.Delete)
        {
            dao.Execute(new DeleteRequest { Target = new EntityReference(Step, step.Id) });
        }

        foreach (var change in plan.Change)
        {
            Apply(change);
        }

        foreach (var step in plan.Create)
        {
            Create(typeId.Value, step, result);
        }

        result.Deleted = plan.Delete.Count;
        result.Updated = plan.Change.Count;
        result.Unchanged = plan.Unchanged;
        return result;
    }

    // The trigger of each published version of an active, enabled rule.
    private List<RuleTrigger> PublishedTriggers()
    {
        var query = new QueryExpression(rs_Rule.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("rs_enabled", "rs_publishedversionid"),
            TopCount = MaxRows,
        };
        query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        query.Criteria.AddCondition("rs_publishedversionid", ConditionOperator.NotNull);
        var rules = dao.RetrieveList<rs_Rule>(query).Where(r => r.rs_Enabled != false).ToList();
        var versionIds = rules.Select(r => r.rs_PublishedVersionId!.Id).ToList();
        return dao.RetrieveMultipleByIds<rs_RuleVersion>(versionIds)
            .Select(v => Json.Read<CompiledRule>(v.rs_CompiledSnapshot)?.Trigger)
            .OfType<RuleTrigger>()
            .ToList();
    }

    private List<ExistingStep> ExistingSteps(Guid typeId)
    {
        var query = new QueryExpression(Step)
        {
            ColumnSet = new ColumnSet(Name, "stage", "mode", FilteringAttributes, AsyncAutoDelete),
            TopCount = MaxRows,
        };
        query.Criteria.AddCondition("plugintypeid", ConditionOperator.Equal, typeId);
        var filter = query.AddLink("sdkmessagefilter", "sdkmessagefilterid", "sdkmessagefilterid", JoinOperator.LeftOuter);
        filter.EntityAlias = "filter";
        filter.Columns = new ColumnSet("primaryobjecttypecode");
        var message = query.AddLink("sdkmessage", "sdkmessageid", "sdkmessageid", JoinOperator.LeftOuter);
        message.EntityAlias = "message";
        message.Columns = new ColumnSet(Name);
        var rows = dao.RetrieveList<Entity>(query);
        var withImage = PreImageSteps(rows.Select(r => r.Id).ToList());
        return rows.Select(r => new ExistingStep
        {
            Id = r.Id,
            Name = r.GetAttributeValue<string>(Name) ?? string.Empty,
            Table = Aliased(r, "filter.primaryobjecttypecode"),
            Message = Aliased(r, "message.name"),
            Stage = r.GetAttributeValue<OptionSetValue>("stage")?.Value ?? 0,
            Mode = r.GetAttributeValue<OptionSetValue>("mode")?.Value ?? 0,
            FilteringAttributes = r.GetAttributeValue<string>(FilteringAttributes),
            AsyncAutoDelete = r.GetAttributeValue<bool>(AsyncAutoDelete),
            HasPreImage = withImage.Contains(r.Id),
        }).ToList();
    }

    // The steps among these that have the engine's pre-image.
    private HashSet<Guid> PreImageSteps(List<Guid> stepIds)
    {
        if (stepIds.Count == 0)
            return [];

        var query = new QueryExpression(StepImage) { ColumnSet = new ColumnSet(StepId), TopCount = MaxRows };
        query.Criteria.AddCondition(StepId, ConditionOperator.In, stepIds.Cast<object>().ToArray());
        query.Criteria.AddCondition("entityalias", ConditionOperator.Equal, PreImageName);
        query.Criteria.AddCondition("imagetype", ConditionOperator.In, 0, 2);
        return new HashSet<Guid>(dao.RetrieveList<Entity>(query).Select(i => i.GetAttributeValue<EntityReference>(StepId)?.Id ?? Guid.Empty));
    }

    private void Apply(StepChange change)
    {
        if (change.UpdateStep)
        {
            var update = new Entity(Step, change.Existing.Id)
            {
                [Name] = change.Desired.Name,
                [FilteringAttributes] = change.Desired.FilteringAttributes.Length == 0 ? null : change.Desired.FilteringAttributes,
            };
            if (change.Desired.Mode == StepPlanner.Asynchronous)
            {
                update[AsyncAutoDelete] = true;
            }

            dao.Execute(new UpdateRequest { Target = update });
        }

        if (change.AddPreImage)
        {
            CreatePreImage(change.Existing.Id);
        }
    }

    private void Create(Guid typeId, DesiredStep step, ReconcileResult result)
    {
        var messageId = MessageId(step.Message);
        var filterId = messageId == null ? null : FilterId(messageId.Value, step.Table);
        if (filterId == null)
        {
            result.Warnings.Add(new ValidationIssue
            {
                Severity = Warning,
                Code = ValidationCodes.StepNotSupported,
                Message = $"Dataverse has no {step.Message} plug-in step for table {step.Table}, so rules on it cannot run.",
            });
            return;
        }

        var entity = new Entity(Step)
        {
            [Name] = step.Name,
            ["description"] = "Created by Rule Studio publish; do not edit. Rebuilt by rs_ReconcileSteps.",
            ["plugintypeid"] = new EntityReference("plugintype", typeId),
            ["sdkmessageid"] = new EntityReference("sdkmessage", messageId!.Value),
            ["sdkmessagefilterid"] = new EntityReference("sdkmessagefilter", filterId.Value),
            ["stage"] = new OptionSetValue(step.Stage),
            ["mode"] = new OptionSetValue(step.Mode),
            ["rank"] = 1,
            ["supporteddeployment"] = new OptionSetValue(0),
            [FilteringAttributes] = step.FilteringAttributes.Length == 0 ? null : step.FilteringAttributes,
            [AsyncAutoDelete] = step.Mode == StepPlanner.Asynchronous,
        };
        var id = ((CreateResponse)dao.Execute(new CreateRequest { Target = entity })).id;
        if (step.PreImage)
        {
            CreatePreImage(id);
        }

        result.Created++;
    }

    // A pre-image of all columns, as the engine reads it (EngineSettings.PreImageName).
    private void CreatePreImage(Guid stepId) =>
        dao.Execute(new CreateRequest
        {
            Target = new Entity(StepImage)
            {
                [StepId] = new EntityReference(Step, stepId),
                ["imagetype"] = new OptionSetValue(0),
                ["entityalias"] = PreImageName,
                [Name] = PreImageName,
                ["messagepropertyname"] = "Target",
            },
        });

    private Guid? MessageId(string message)
    {
        var query = new QueryExpression("sdkmessage") { ColumnSet = new ColumnSet("sdkmessageid"), TopCount = 1 };
        query.Criteria.AddCondition(Name, ConditionOperator.Equal, message);
        return dao.RetrieveList<Entity>(query).FirstOrDefault()?.Id;
    }

    private Guid? FilterId(Guid messageId, string table)
    {
        var query = new QueryExpression("sdkmessagefilter") { ColumnSet = new ColumnSet("sdkmessagefilterid"), TopCount = 1 };
        query.Criteria.AddCondition("sdkmessageid", ConditionOperator.Equal, messageId);
        query.Criteria.AddCondition("primaryobjecttypecode", ConditionOperator.Equal, table);
        query.Criteria.AddCondition("iscustomprocessingstepallowed", ConditionOperator.Equal, true);
        return dao.RetrieveList<Entity>(query).FirstOrDefault()?.Id;
    }

    private static string Aliased(Entity entity, string name) => entity.GetAttributeValue<AliasedValue>(name)?.Value as string ?? string.Empty;
}
