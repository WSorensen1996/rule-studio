using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Standard custom function: side effects, not safe to retry. Value = the HTTP status code.</summary>
public class CallWebhook : Plugin
{
    public CallWebhook()
    {
        RegisterAPI<StandardFunctionsApi>(nameof(CallWebhook), nameof(StandardFunctionsApi.CallWebhook))
            .AddRequestParameter("Url", CustomApiParameterType.String, isOptional: true)
            .AddRequestParameter("Payload", CustomApiParameterType.String, isOptional: true)
            .AddRequestParameter("AuthorizationVariable", CustomApiParameterType.String, isOptional: true)
            .AddResponseProperty("Success", CustomApiParameterType.Boolean)
            .AddResponseProperty("Value", CustomApiParameterType.Integer)
            .AddResponseProperty("Message", CustomApiParameterType.String);
    }
}
