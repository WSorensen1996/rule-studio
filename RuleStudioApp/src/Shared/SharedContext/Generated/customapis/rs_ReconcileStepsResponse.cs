namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("rs_ReconcileSteps")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_ReconcileStepsResponse : Microsoft.Xrm.Sdk.OrganizationResponse
{
    public rs_ReconcileStepsResponse()
    {
    }

    public rs_ReconcileStepsResponse(string summary)
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
