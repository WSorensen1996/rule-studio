namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("rs_ValidatePhone")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_ValidatePhoneRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
    public rs_ValidatePhoneRequest()
    {
        this.RequestName = "rs_ValidatePhone";
    }

    
    public string? Phone
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("Phone", out var value))
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
            this.Parameters["Phone"] = value;
        }
    }

    
    public string? CountryCode
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("CountryCode", out var value))
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
            this.Parameters["CountryCode"] = value;
        }
    }
}
