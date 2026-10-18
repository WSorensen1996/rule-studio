using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Standard custom function: side effects, not safe to retry. Value = the new task.</summary>
public class CreateTask : Plugin
{
    public CreateTask()
    {
        RegisterAPI<StandardFunctionsApi>(nameof(CreateTask), nameof(StandardFunctionsApi.CreateTask))
            .AddRequestParameter("RegardingTable", CustomApiParameterType.String, isOptional: true)
            .AddRequestParameter("RegardingId", CustomApiParameterType.String, isOptional: true)
            .AddRequestParameter("Subject", CustomApiParameterType.String, isOptional: true)
            .AddRequestParameter("Description", CustomApiParameterType.String, isOptional: true)
            .AddRequestParameter("DueInDays", CustomApiParameterType.Integer, isOptional: true)
            .AddRequestParameter("OwnerId", CustomApiParameterType.EntityReference, isOptional: true)
            .AddResponseProperty("Success", CustomApiParameterType.Boolean)
            .AddResponseProperty("Value", CustomApiParameterType.EntityReference)
            .AddResponseProperty("Message", CustomApiParameterType.String);
    }
}
