namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("rs_PublishRelease")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_PublishReleaseRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
    public rs_PublishReleaseRequest()
    {
        this.RequestName = "rs_PublishRelease";
        this.RuleIds = default(string);
        this.Name = default(string);
    }

    
    public string RuleIds
    {
        get
        {
            if (this.Parameters.TryGetValue<string>("RuleIds", out var value))
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
            this.Parameters["RuleIds"] = value;
        }
    }

    
    public string Name
    {
        get
        {
            if (this.Parameters.TryGetValue<string>("Name", out var value))
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
            this.Parameters["Name"] = value;
        }
    }

    
    public string? Description
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("Description", out var value))
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
            this.Parameters["Description"] = value;
        }
    }
}
