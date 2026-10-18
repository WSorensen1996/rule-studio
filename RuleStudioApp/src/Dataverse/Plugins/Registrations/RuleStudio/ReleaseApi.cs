using DataverseLogic;
using DataverseLogic.RuleStudio.Release;
using DataverseLogic.RuleStudio.Simulation;
using DataverseLogic.RuleStudio.Steps;

namespace DataverseRegistration.RuleStudio;

/// <summary>Maps the generated Custom API requests and responses onto <see cref="ReleaseService"/>, <see cref="RollbackService"/>, <see cref="RestoreService"/>, <see cref="SimulateService"/> and <see cref="StepReconciler"/>.</summary>
public class ReleaseApi(ReleaseService service, RollbackService rollbackService, RestoreService restoreService, SimulateService simulateService, StepReconciler stepReconciler)
{
    private readonly ReleaseService service = service;
    private readonly RollbackService rollbackService = rollbackService;
    private readonly RestoreService restoreService = restoreService;
    private readonly SimulateService simulateService = simulateService;
    private readonly StepReconciler stepReconciler = stepReconciler;

    public ValidateReleaseResponse Validate(ValidateReleaseRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        return new ValidateReleaseResponse(service.Validate(request.RuleIds ?? string.Empty));
    }

    public PublishReleaseResponse Publish(PublishReleaseRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        var (validation, releaseId) = service.Publish(request.RuleIds ?? string.Empty, request.Name ?? string.Empty, request.Description);
        return new PublishReleaseResponse(validation, releaseId);
    }

    public RollbackReleaseResponse Rollback(RollbackReleaseRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        return new RollbackReleaseResponse(rollbackService.Rollback(request.ReleaseId));
    }

    public RestoreVersionResponse Restore(RestoreVersionRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        return new RestoreVersionResponse(restoreService.Restore(request.RuleVersionId, request.ReplaceDraft ?? false));
    }

    public ReconcileStepsResponse Reconcile() => new(stepReconciler.Reconcile().Summary());

    public SimulateRuleResponse Simulate(SimulateRuleRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        return new SimulateRuleResponse(simulateService.Simulate(new SimulationRequest
        {
            RuleIds = request.RuleIds,
            Table = request.Table,
            Event = request.Event,
            Stage = request.Stage,
            UseDrafts = request.PublishedOnly != true,
            RecordId = request.RecordId,
            SampleValues = request.SampleValues,
            MockFunctions = request.CallFunctions != true,
            LookupDefinitionId = request.LookupDefinitionId,
        }));
    }
}
