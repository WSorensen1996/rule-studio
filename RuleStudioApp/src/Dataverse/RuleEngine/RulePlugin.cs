using DataverseLogic;
using Microsoft.Xrm.Sdk;

namespace RuleStudio.Engine;

/// <summary>
/// The one generic Rule Studio plug-in. It finds the published rules of the
/// current table, event and stage and runs them. Its steps are created by the step reconciler at
/// publish time, never by XrmSync: Update and Delete steps carry a pre-image named "PreImage";
/// before-save steps are synchronous PreOperation, after-save steps asynchronous PostOperation.
/// </summary>
public sealed class RulePlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(serviceProvider, nameof(serviceProvider));
        new EngineRun(Service<IPluginExecutionContext>(serviceProvider), Service<IOrganizationServiceFactory>(serviceProvider), Service<ITracingService>(serviceProvider)).Execute();
    }

    private static T Service<T>(IServiceProvider serviceProvider)
        where T : class =>
        serviceProvider.GetService(typeof(T)) as T ?? throw new InvalidPluginExecutionException($"Rule Studio: the platform did not provide {typeof(T).Name}.");
}
