using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Standard custom function: pure, safe to retry. Value = whether the VAT number is valid (format and check digits, offline).</summary>
public class ValidateVatNumber : Plugin
{
    public ValidateVatNumber()
    {
        RegisterAPI<StandardFunctionsApi>(nameof(ValidateVatNumber), nameof(StandardFunctionsApi.ValidateVatNumber))
            .AddRequestParameter("VatNumber", CustomApiParameterType.String, isOptional: true)
            .AddRequestParameter("CountryCode", CustomApiParameterType.String, isOptional: true)
            .AddResponseProperty("Success", CustomApiParameterType.Boolean)
            .AddResponseProperty("Value", CustomApiParameterType.Boolean)
            .AddResponseProperty("Message", CustomApiParameterType.String);
    }
}
