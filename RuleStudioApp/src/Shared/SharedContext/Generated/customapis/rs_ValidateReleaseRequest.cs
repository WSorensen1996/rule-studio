namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("rs_ValidateRelease")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_ValidateReleaseRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
    public rs_ValidateReleaseRequest()
    {
        this.RequestName = "rs_ValidateRelease";
        this.RuleIds = default(string);
    }

    
    public string RuleIds
    {
        get
        {
            if (this.Parameters.TryGetValue<string>("RuleIds", out var value))
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
            this.Parameters["RuleIds"] = value;
        }
    }
}
