namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("rs_RollbackRelease")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_RollbackReleaseResponse : Microsoft.Xrm.Sdk.OrganizationResponse
{
    public rs_RollbackReleaseResponse()
    {
    }

    public rs_RollbackReleaseResponse(System.Guid releaseId)
    {
        this.ReleaseId = releaseId;
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
