namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("rs_ValidateRelease")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_ValidateReleaseResponse : Microsoft.Xrm.Sdk.OrganizationResponse
{
    public rs_ValidateReleaseResponse()
    {
    }

    public rs_ValidateReleaseResponse(string validationResult)
    {
        this.ValidationResult = validationResult;
    }


    
    public string ValidationResult
    {
        get
        {
            if (this.Results.TryGetValue<string>("ValidationResult", out var value))
            {
                return value;
            }
            else
            {
                return default(string);
            }
        }
        set
        {
            this.Results["ValidationResult"] = value;
        }
    }
}
