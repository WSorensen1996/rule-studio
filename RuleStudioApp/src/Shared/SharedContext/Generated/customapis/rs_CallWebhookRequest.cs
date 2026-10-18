namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("rs_CallWebhook")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_CallWebhookRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
    public rs_CallWebhookRequest()
    {
        this.RequestName = "rs_CallWebhook";
    }

    
    public string? Url
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("Url", out var value))
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
            this.Parameters["Url"] = value;
        }
    }

    
    public string? Payload
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("Payload", out var value))
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
            this.Parameters["Payload"] = value;
        }
    }

    
    public string? AuthorizationVariable
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("AuthorizationVariable", out var value))
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
            this.Parameters["AuthorizationVariable"] = value;
        }
    }
}
