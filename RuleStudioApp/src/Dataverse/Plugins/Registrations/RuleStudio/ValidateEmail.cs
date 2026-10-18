using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Standard custom function: pure, safe to retry. Value = whether the e-mail address is valid.</summary>
public class ValidateEmail : Plugin
{
    public ValidateEmail()
    {
        RegisterAPI<StandardFunctionsApi>(nameof(ValidateEmail), nameof(StandardFunctionsApi.ValidateEmail))
            .AddRequestParameter("Email", CustomApiParameterType.String, isOptional: true)
            .AddResponseProperty("Success", CustomApiParameterType.Boolean)
            .AddResponseProperty("Value", CustomApiParameterType.Boolean)
            .AddResponseProperty("Message", CustomApiParameterType.String);
    }
}
