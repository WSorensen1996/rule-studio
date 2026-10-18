namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("rs_ExportRules")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_ExportRulesRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
    public rs_ExportRulesRequest()
    {
        this.RequestName = "rs_ExportRules";
    }

    
    public string? RuleKeys
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("RuleKeys", out var value))
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
            this.Parameters["RuleKeys"] = value;
        }
    }
}
