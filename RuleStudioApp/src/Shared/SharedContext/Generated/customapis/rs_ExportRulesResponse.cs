namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("rs_ExportRules")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_ExportRulesResponse : Microsoft.Xrm.Sdk.OrganizationResponse
{
    public rs_ExportRulesResponse()
    {
    }

    public rs_ExportRulesResponse(string bundle)
    {
        this.Bundle = bundle;
    }


    
    public string Bundle
    {
        get
        {
            if (this.Results.TryGetValue<string>("Bundle", out var value))
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
            this.Results["Bundle"] = value;
        }
    }
}
