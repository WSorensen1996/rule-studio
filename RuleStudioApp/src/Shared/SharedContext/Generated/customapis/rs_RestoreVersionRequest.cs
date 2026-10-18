namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("rs_RestoreVersion")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_RestoreVersionRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
    public rs_RestoreVersionRequest()
    {
        this.RequestName = "rs_RestoreVersion";
        this.RuleVersionId = default(System.Guid);
    }

    
    public System.Guid RuleVersionId
    {
        get
        {
            if (this.Parameters.TryGetValue<System.Guid>("RuleVersionId", out var value))
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
            this.Parameters["RuleVersionId"] = value;
        }
    }

    
    public bool? ReplaceDraft
    {
        get
        {
            if (this.Parameters.TryGetValue<bool?>("ReplaceDraft", out var value))
            {
                return value;
            }
            else
            {
                return default(bool?);
            }
        }
        set
        {
            this.Parameters["ReplaceDraft"] = value;
        }
    }
}
