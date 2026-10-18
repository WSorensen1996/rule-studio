using DataverseLogic;
using DataverseLogic.RuleStudio.Operations;

namespace DataverseRegistration.RuleStudio;

/// <summary>Maps the generated Custom API requests and responses onto <see cref="RetentionService"/>.</summary>
public class OperationsApi(RetentionService retentionService)
{
    private readonly RetentionService retentionService = retentionService;

    public ConfigureLogRetentionResponse ConfigureRetention(ConfigureLogRetentionRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        return new ConfigureLogRetentionResponse(retentionService.Configure(request.Days, request.ErrorDays));
    }
}
