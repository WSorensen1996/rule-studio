namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("rs_ValidateVatNumber")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_ValidateVatNumberRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
    public rs_ValidateVatNumberRequest()
    {
        this.RequestName = "rs_ValidateVatNumber";
    }

    
    public string? VatNumber
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("VatNumber", out var value))
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
            this.Parameters["VatNumber"] = value;
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
