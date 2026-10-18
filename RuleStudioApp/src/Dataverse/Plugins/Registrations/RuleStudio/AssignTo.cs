using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Standard custom function: side effects, safe to retry. Value = true when the record was assigned.</summary>
public class AssignTo : Plugin
{
    public AssignTo()
    {
        RegisterAPI<StandardFunctionsApi>(nameof(AssignTo), nameof(StandardFunctionsApi.AssignTo))
            .AddRequestParameter("RecordTable", CustomApiParameterType.String, isOptional: true)
            .AddRequestParameter("RecordId", CustomApiParameterType.String, isOptional: true)
            .AddRequestParameter("OwnerId", CustomApiParameterType.EntityReference, isOptional: true)
            .AddResponseProperty("Success", CustomApiParameterType.Boolean)
            .AddResponseProperty("Value", CustomApiParameterType.Boolean)
            .AddResponseProperty("Message", CustomApiParameterType.String);
    }
}
