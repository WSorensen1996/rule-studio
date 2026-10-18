namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("rs_ConfigureLogRetention")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_ConfigureLogRetentionRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
    public rs_ConfigureLogRetentionRequest()
    {
        this.RequestName = "rs_ConfigureLogRetention";
    }

    
    public int? Days
    {
        get
        {
            if (this.Parameters.TryGetValue<int?>("Days", out var value))
            {
                return value;
            }
            else
            {
                return default(int?);
            }
        }
        set
        {
            this.Parameters["Days"] = value;
        }
    }

    
    public int? ErrorDays
    {
        get
        {
            if (this.Parameters.TryGetValue<int?>("ErrorDays", out var value))
            {
                return value;
            }
            else
            {
                return default(int?);
            }
        }
        set
        {
            this.Parameters["ErrorDays"] = value;
        }
    }
}
