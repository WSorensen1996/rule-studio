namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("rs_RestoreVersion")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_RestoreVersionResponse : Microsoft.Xrm.Sdk.OrganizationResponse
{
    public rs_RestoreVersionResponse()
    {
    }

    public rs_RestoreVersionResponse(System.Guid ruleVersionId)
    {
        this.RuleVersionId = ruleVersionId;
    }


    
    public System.Guid RuleVersionId
    {
        get
        {
            if (this.Results.TryGetValue<System.Guid>("RuleVersionId", out var value))
            {
                return value;
            }
            else
            {
                return default(System.Guid);
            }
        }
        set
        {
            this.Results["RuleVersionId"] = value;
        }
    }
}
