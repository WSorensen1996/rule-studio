using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Rebuilds the engine plug-in steps from the published rules (docs/custom-apis.md, docs/engine.md).</summary>
public class ReconcileSteps : Plugin
{
    public ReconcileSteps()
    {
        RegisterAPI<ReleaseApi>(nameof(ReconcileSteps), nameof(ReleaseApi.Reconcile))
            .AddResponseProperty("Summary", CustomApiParameterType.String);
    }
}
