namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("rs_ValidateEmail")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_ValidateEmailRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
    public rs_ValidateEmailRequest()
    {
        this.RequestName = "rs_ValidateEmail";
    }

    
    public string? Email
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("Email", out var value))
            {
                return value;
            }
            else
            {
                return default(string?);
            }
        }
        set
        {
            this.Parameters["Email"] = value;
        }
    }
}
