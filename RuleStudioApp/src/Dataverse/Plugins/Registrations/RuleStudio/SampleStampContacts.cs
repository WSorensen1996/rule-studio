using XrmPluginCore.Enums;

namespace DataverseRegistration.RuleStudio;

/// <summary>Sample custom function: side effects, not safe to retry. Writes description on the account's contacts.</summary>
public class SampleStampContacts : Plugin
{
    public SampleStampContacts()
    {
        RegisterAPI<SampleApi>(nameof(SampleStampContacts), nameof(SampleApi.StampContacts))
            .AddRequestParameter("AccountId", CustomApiParameterType.EntityReference, isOptional: true)
            .AddRequestParameter("Text", CustomApiParameterType.String, isOptional: true)
            .AddResponseProperty("Success", CustomApiParameterType.Boolean)
            .AddResponseProperty("Value", CustomApiParameterType.Integer)
            .AddResponseProperty("Message", CustomApiParameterType.String);
    }
}
