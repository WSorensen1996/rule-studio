using DataverseLogic;
using DataverseLogic.RuleStudio.Functions;

namespace DataverseRegistration.RuleStudio;

/// <summary>Maps the generated Custom API requests and responses onto the standard custom functions.</summary>
public class StandardFunctionsApi(RecordFunctions records, WebhookFunction webhook)
{
    private readonly RecordFunctions records = records;
    private readonly WebhookFunction webhook = webhook;

#pragma warning disable CA1822 // XrmPluginCore calls Custom API handlers on the DI instance.
    public ValidateEmailResponse ValidateEmail(ValidateEmailRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        var (success, value, message) = ValidationFunctions.ValidateEmail(request.Email);
        return new ValidateEmailResponse(success, value, message);
    }

    public ValidatePhoneResponse ValidatePhone(ValidatePhoneRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        var (success, value, message) = ValidationFunctions.ValidatePhone(request.Phone, request.CountryCode);
        return new ValidatePhoneResponse(success, value, message);
    }

    public ValidateVatNumberResponse ValidateVatNumber(ValidateVatNumberRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        var (success, value, message) = ValidationFunctions.ValidateVatNumber(request.VatNumber, request.CountryCode);
        return new ValidateVatNumberResponse(success, value, message);
    }
#pragma warning restore CA1822

    public CreateTaskResponse CreateTask(CreateTaskRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        var (success, value, message) = records.CreateTask(request.RegardingTable, request.RegardingId, request.Subject, request.Description, request.DueInDays, request.OwnerId);
        return new CreateTaskResponse(success, value!, message);
    }

    public AssignToResponse AssignTo(AssignToRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        var (success, value, message) = records.AssignTo(request.RecordTable, request.RecordId, request.OwnerId);
        return new AssignToResponse(success, value, message);
    }

    public CallWebhookResponse CallWebhook(CallWebhookRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        var (success, value, message) = webhook.CallWebhook(request.Url, request.Payload, request.AuthorizationVariable);
        return new CallWebhookResponse(success, value, message);
    }
}
