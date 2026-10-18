using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Validates the drafts of the given rules and publishes them as one release (docs/custom-apis.md).</summary>
public class PublishRelease : Plugin
{
    public PublishRelease()
    {
        RegisterAPI<ReleaseApi>(nameof(PublishRelease), nameof(ReleaseApi.Publish))
            .AddRequestParameter("RuleIds", CustomApiParameterType.String)
            .AddRequestParameter("Name", CustomApiParameterType.String)
            .AddRequestParameter("Description", CustomApiParameterType.String, isOptional: true)
            .AddResponseProperty("ValidationResult", CustomApiParameterType.String)
            .AddResponseProperty("ReleaseId", CustomApiParameterType.Guid);
    }
}
