using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Puts every rule of a live release back on its previous live version, as a new release (docs/custom-apis.md).</summary>
public class RollbackRelease : Plugin
{
    public RollbackRelease()
    {
        RegisterAPI<ReleaseApi>(nameof(RollbackRelease), nameof(ReleaseApi.Rollback))
            .AddRequestParameter("ReleaseId", CustomApiParameterType.Guid)
            .AddResponseProperty("ReleaseId", CustomApiParameterType.Guid);
    }
}
