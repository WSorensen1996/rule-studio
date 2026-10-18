namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("rs_SimulateRule")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_SimulateRuleRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
    public rs_SimulateRuleRequest()
    {
        this.RequestName = "rs_SimulateRule";
    }

    
    public string? RuleIds
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("RuleIds", out var value))
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
            this.Parameters["RuleIds"] = value;
        }
    }

    
    public string? Table
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("Table", out var value))
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
            this.Parameters["Table"] = value;
        }
    }

    
    public string? @Event
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("Event", out var value))
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
            this.Parameters["Event"] = value;
        }
    }

    
    public string? Stage
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("Stage", out var value))
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
            this.Parameters["Stage"] = value;
        }
    }

    
    public bool? PublishedOnly
    {
        get
        {
            if (this.Parameters.TryGetValue<bool?>("PublishedOnly", out var value))
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
            this.Parameters["PublishedOnly"] = value;
        }
    }

    
    public System.Guid? RecordId
    {
        get
        {
            if (this.Parameters.TryGetValue<System.Guid?>("RecordId", out var value))
            {
                return value;
            }
            else
            {
                return default(System.Guid?);
            }
        }
        set
        {
            this.Parameters["RecordId"] = value;
        }
    }

    
    public string? SampleValues
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("SampleValues", out var value))
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
            this.Parameters["SampleValues"] = value;
        }
    }

    
    public bool? CallFunctions
    {
        get
        {
            if (this.Parameters.TryGetValue<bool?>("CallFunctions", out var value))
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
            this.Parameters["CallFunctions"] = value;
        }
    }

    
    public System.Guid? LookupDefinitionId
    {
        get
        {
            if (this.Parameters.TryGetValue<System.Guid?>("LookupDefinitionId", out var value))
            {
                return value;
            }
            else
            {
                return default(System.Guid?);
            }
        }
        set
        {
            this.Parameters["LookupDefinitionId"] = value;
        }
    }
}
