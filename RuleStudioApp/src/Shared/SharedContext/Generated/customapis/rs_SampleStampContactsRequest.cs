namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.RequestProxyAttribute("rs_SampleStampContacts")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_SampleStampContactsRequest : Microsoft.Xrm.Sdk.OrganizationRequest
{
    public rs_SampleStampContactsRequest()
    {
        this.RequestName = "rs_SampleStampContacts";
    }

    
    public Microsoft.Xrm.Sdk.EntityReference? AccountId
    {
        get
        {
            if (this.Parameters.TryGetValue<Microsoft.Xrm.Sdk.EntityReference?>("AccountId", out var value))
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
            this.Parameters["AccountId"] = value;
        }
    }

    
    public string? Text
    {
        get
        {
            if (this.Parameters.TryGetValue<string?>("Text", out var value))
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
            this.Parameters["Text"] = value;
        }
    }
}
