using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Exports the published rules as a portable bundle (docs/custom-apis.md).</summary>
public class ExportRules : Plugin
{
    public ExportRules()
    {
        RegisterAPI<PromotionApi>(nameof(ExportRules), nameof(PromotionApi.Export))
            .AddRequestParameter("RuleKeys", CustomApiParameterType.String, isOptional: true)
            .AddResponseProperty("Bundle", CustomApiParameterType.String);
    }
}
