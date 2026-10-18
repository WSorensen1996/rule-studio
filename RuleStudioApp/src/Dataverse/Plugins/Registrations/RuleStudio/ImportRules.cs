using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Checks or applies a rule bundle with the three-way merge (docs/custom-apis.md).</summary>
public class ImportRules : Plugin
{
    public ImportRules()
    {
        RegisterAPI<PromotionApi>(nameof(ImportRules), nameof(PromotionApi.Import))
            .AddRequestParameter("Bundle", CustomApiParameterType.String)
            .AddRequestParameter("Mode", CustomApiParameterType.String, isOptional: true)
            .AddResponseProperty("Report", CustomApiParameterType.String);
    }
}
