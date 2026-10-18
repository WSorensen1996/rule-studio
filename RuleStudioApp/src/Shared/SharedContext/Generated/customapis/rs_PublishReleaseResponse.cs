namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("rs_PublishRelease")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_PublishReleaseResponse : Microsoft.Xrm.Sdk.OrganizationResponse
{
    public rs_PublishReleaseResponse()
    {
    }

    public rs_PublishReleaseResponse(string validationResult, System.Guid releaseId)
    {
        this.ValidationResult = validationResult;
        this.ReleaseId = releaseId;
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

    
    public System.Guid ReleaseId
    {
        get
        {
            if (this.Results.TryGetValue<System.Guid>("ReleaseId", out var value))
            {
                return value;
            }
            else
            {
                return default(System.Guid);
            }
        }
        set
        {
            this.Results["ReleaseId"] = value;
        }
    }
}
