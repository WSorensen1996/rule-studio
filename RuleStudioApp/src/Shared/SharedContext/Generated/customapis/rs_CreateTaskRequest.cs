namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("rs_CreateTask")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_CreateTaskRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
    public rs_CreateTaskRequest()
    {
        this.RequestName = "rs_CreateTask";
    }

    
    public string? RegardingTable
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("RegardingTable", out var value))
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
            this.Parameters["RegardingTable"] = value;
        }
    }

    
    public string? RegardingId
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("RegardingId", out var value))
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
            this.Parameters["RegardingId"] = value;
        }
    }

    
    public string? Subject
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("Subject", out var value))
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
            this.Parameters["Subject"] = value;
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

    
    public int? DueInDays
    {
        get
        {
            if (this.Parameters.TryGetValue<int?>("DueInDays", out var value))
            {
                return value;
            }
            else
            {
                return default(int?);
            }
        }
        set
        {
            this.Parameters["DueInDays"] = value;
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
