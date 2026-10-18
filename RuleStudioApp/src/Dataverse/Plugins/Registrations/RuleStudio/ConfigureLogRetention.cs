using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Schedules the daily bulk-delete jobs that keep rs_executionlog small (docs/custom-apis.md).</summary>
public class ConfigureLogRetention : Plugin
{
    public ConfigureLogRetention()
    {
        RegisterAPI<OperationsApi>(nameof(ConfigureLogRetention), nameof(OperationsApi.ConfigureRetention))
            .AddRequestParameter("Days", CustomApiParameterType.Integer, isOptional: true)
            .AddRequestParameter("ErrorDays", CustomApiParameterType.Integer, isOptional: true)
            .AddResponseProperty("Summary", CustomApiParameterType.String);
    }
}
