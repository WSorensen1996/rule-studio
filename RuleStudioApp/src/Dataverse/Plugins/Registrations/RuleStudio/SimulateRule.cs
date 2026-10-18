using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Runs rules against a real or sample record and returns the traces. Saves nothing (docs/custom-apis.md).</summary>
public class SimulateRule : Plugin
{
    public SimulateRule()
    {
        RegisterAPI<ReleaseApi>(nameof(SimulateRule), nameof(ReleaseApi.Simulate))
            .AddRequestParameter("RuleIds", CustomApiParameterType.String, isOptional: true)
            .AddRequestParameter("Table", CustomApiParameterType.String, isOptional: true)
            .AddRequestParameter("Event", CustomApiParameterType.String, isOptional: true)
            .AddRequestParameter("Stage", CustomApiParameterType.String, isOptional: true)
            .AddRequestParameter("PublishedOnly", CustomApiParameterType.Boolean, isOptional: true)
            .AddRequestParameter("RecordId", CustomApiParameterType.Guid, isOptional: true)
            .AddRequestParameter("SampleValues", CustomApiParameterType.String, isOptional: true)
            .AddRequestParameter("CallFunctions", CustomApiParameterType.Boolean, isOptional: true)
            .AddRequestParameter("LookupDefinitionId", CustomApiParameterType.Guid, isOptional: true)
            .AddResponseProperty("Trace", CustomApiParameterType.String);
    }
}
