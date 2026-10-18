using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Standard custom function: pure, safe to retry. Value = whether the phone number is valid.</summary>
public class ValidatePhone : Plugin
{
    public ValidatePhone()
    {
        RegisterAPI<StandardFunctionsApi>(nameof(ValidatePhone), nameof(StandardFunctionsApi.ValidatePhone))
            .AddRequestParameter("Phone", CustomApiParameterType.String, isOptional: true)
            .AddRequestParameter("CountryCode", CustomApiParameterType.String, isOptional: true)
            .AddResponseProperty("Success", CustomApiParameterType.Boolean)
            .AddResponseProperty("Value", CustomApiParameterType.Boolean)
            .AddResponseProperty("Message", CustomApiParameterType.String);
    }
}
