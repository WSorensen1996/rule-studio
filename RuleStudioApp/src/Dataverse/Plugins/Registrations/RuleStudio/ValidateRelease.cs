using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Runs the publish checks on the drafts of the given rules. Saves nothing (docs/custom-apis.md).</summary>
public class ValidateRelease : Plugin
{
    public ValidateRelease()
    {
        RegisterAPI<ReleaseApi>(nameof(ValidateRelease), nameof(ReleaseApi.Validate))
            .AddRequestParameter("RuleIds", CustomApiParameterType.String)
            .AddResponseProperty("ValidationResult", CustomApiParameterType.String);
    }
}
