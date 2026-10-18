namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("rs_SimulateRule")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_SimulateRuleResponse : Microsoft.Xrm.Sdk.OrganizationResponse
{
    public rs_SimulateRuleResponse()
    {
    }

    public rs_SimulateRuleResponse(string trace)
    {
        this.Trace = trace;
    }


    
    public string Trace
    {
        get
        {
            if (this.Results.TryGetValue<string>("Trace", out var value))
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
            this.Results["Trace"] = value;
        }
    }
}
