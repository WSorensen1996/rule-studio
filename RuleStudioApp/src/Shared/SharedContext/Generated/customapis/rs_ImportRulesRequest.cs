namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("rs_ImportRules")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_ImportRulesRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
    public rs_ImportRulesRequest()
    {
        this.RequestName = "rs_ImportRules";
        this.Bundle = default(string);
    }

    
    public string Bundle
    {
        get
        {
            if (this.Parameters.TryGetValue<string>("Bundle", out var value))
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
            this.Parameters["Bundle"] = value;
        }
    }

    
    public string? Mode
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("Mode", out var value))
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
            this.Parameters["Mode"] = value;
        }
    }
}
