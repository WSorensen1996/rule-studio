using DataverseLogic;
using DataverseLogic.RuleStudio.Functions;
using DataverseLogic.RuleStudio.Operations;
using DataverseLogic.RuleStudio.Promotion;
using DataverseLogic.RuleStudio.Release;
using DataverseLogic.RuleStudio.Samples;
using DataverseLogic.RuleStudio.Simulation;
using DataverseLogic.RuleStudio.Steps;
using Microsoft.Extensions.DependencyInjection;
using SharedContext.Dao;

namespace DataverseRegistration;

internal static class PluginSetup
{
    internal static void SetupCustomDependencies(this IServiceCollection services)
    {
        // Dao objects
        services.AddScoped<IAdminDataverseAccessObjectService, AdminDataverseAccessObjectService>();
        services.AddScoped<IUserDataverseAccessObjectService, UserDataverseAccessObjectService>();

        // Dataverse Logic (lexicografical order please)
        services.AddScoped<ExportService>();
        services.AddScoped<ImportService>();
        services.AddScoped<RecordFunctions>();
        services.AddScoped<ReleaseService>();
        services.AddScoped<RetentionService>();
        services.AddScoped<SampleFunctions>();
        services.AddScoped<RollbackService>();
        services.AddScoped<RestoreService>();
        services.AddScoped<SimulateService>();
        services.AddScoped<StepReconciler>();
        services.AddScoped<WebhookFunction>();
        services.AddSingleton<IWebhookSender, HttpWebhookSender>();
        services.AddScoped<RuleStudio.OperationsApi>();
        services.AddScoped<RuleStudio.PromotionApi>();
        services.AddScoped<RuleStudio.ReleaseApi>();
        services.AddScoped<RuleStudio.SampleApi>();
        services.AddScoped<RuleStudio.StandardFunctionsApi>();
    }
}
