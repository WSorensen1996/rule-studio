namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("rs_ConfigureLogRetention")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_ConfigureLogRetentionResponse : Microsoft.Xrm.Sdk.OrganizationResponse
{
    public rs_ConfigureLogRetentionResponse()
    {
    }

    public rs_ConfigureLogRetentionResponse(string summary)
    {
        this.Summary = summary;
    }


    
    public string Summary
    {
        get
        {
            if (this.Results.TryGetValue<string>("Summary", out var value))
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
            this.Results["Summary"] = value;
        }
    }
}
