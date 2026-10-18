namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("rs_AssignTo")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_AssignToRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
    public rs_AssignToRequest()
    {
        this.RequestName = "rs_AssignTo";
    }

    
    public string? RecordTable
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("RecordTable", out var value))
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
            this.Parameters["RecordTable"] = value;
        }
    }

    
    public string? RecordId
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("RecordId", out var value))
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
            this.Parameters["RecordId"] = value;
        }
    }

    
    public Microsoft.Xrm.Sdk.EntityReference? OwnerId
    {
        get
        {
            if (this.Parameters.TryGetValue<Microsoft.Xrm.Sdk.EntityReference?>("OwnerId", out var value))
            {
                return value;
            }
            else
            {
                return default(Microsoft.Xrm.Sdk.EntityReference?);
            }
        }
        set
        {
            this.Parameters["OwnerId"] = value;
        }
    }
}
