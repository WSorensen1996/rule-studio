using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Copies an earlier rule version into a new draft of the same rule (docs/custom-apis.md).</summary>
public class RestoreVersion : Plugin
{
    public RestoreVersion()
    {
        RegisterAPI<ReleaseApi>(nameof(RestoreVersion), nameof(ReleaseApi.Restore))
            .AddRequestParameter("RuleVersionId", CustomApiParameterType.Guid)
            .AddRequestParameter("ReplaceDraft", CustomApiParameterType.Boolean, isOptional: true)
            .AddResponseProperty("RuleVersionId", CustomApiParameterType.Guid);
    }
}
