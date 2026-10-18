namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("rs_ImportRules")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_ImportRulesResponse : Microsoft.Xrm.Sdk.OrganizationResponse
{
    public rs_ImportRulesResponse()
    {
    }

    public rs_ImportRulesResponse(string report)
    {
        this.Report = report;
    }


    
    public string Report
    {
        get
        {
            if (this.Results.TryGetValue<string>("Report", out var value))
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
            this.Results["Report"] = value;
        }
    }
}
