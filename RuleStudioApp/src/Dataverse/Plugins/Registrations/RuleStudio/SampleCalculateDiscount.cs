using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Sample custom function: pure, safe to retry. Success = false on negative input.</summary>
public class SampleCalculateDiscount : Plugin
{
    public SampleCalculateDiscount()
    {
        RegisterAPI<SampleApi>(nameof(SampleCalculateDiscount), nameof(SampleApi.CalculateDiscount))
            .AddRequestParameter("Revenue", CustomApiParameterType.Decimal, isOptional: true)
            .AddRequestParameter("Employees", CustomApiParameterType.Integer, isOptional: true)
            .AddResponseProperty("Success", CustomApiParameterType.Boolean)
            .AddResponseProperty("Value", CustomApiParameterType.Decimal)
            .AddResponseProperty("Message", CustomApiParameterType.String);
    }
}
