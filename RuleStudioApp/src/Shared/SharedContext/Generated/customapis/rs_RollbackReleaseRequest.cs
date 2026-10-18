namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("rs_RollbackRelease")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_RollbackReleaseRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
    public rs_RollbackReleaseRequest()
    {
        this.RequestName = "rs_RollbackRelease";
        this.ReleaseId = default(System.Guid);
    }

    
    public System.Guid ReleaseId
    {
        get
        {
            if (this.Parameters.TryGetValue<System.Guid>("ReleaseId", out var value))
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
            this.Parameters["ReleaseId"] = value;
        }
    }
}
