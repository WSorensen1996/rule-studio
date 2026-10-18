namespace RuleStudio.SharedContext;

[System.Runtime.Serialization.DataContractAttribute(Namespace="http://schemas.microsoft.com/xrm/2011/new/")]
[Microsoft.Xrm.Sdk.Client.ResponseProxyAttribute("rs_SampleCalculateDiscount")]
[System.CodeDom.Compiler.GeneratedCodeAttribute("DataverseProxyGenerator", "4.0.0.0")]
public partial class rs_SampleCalculateDiscountResponse : Microsoft.Xrm.Sdk.OrganizationResponse
{
    public rs_SampleCalculateDiscountResponse()
    {
    }

    public rs_SampleCalculateDiscountResponse(bool success, decimal value, string message)
    {
        this.Success = success;
        this.Value = value;
        this.Message = message;
    }


    
    public bool Success
    {
        get
        {
            if (this.Results.TryGetValue<bool>("Success", out var value))
            {
                return value;
            }
            else
            {
                return default(bool);
            }
        }
        set
        {
            this.Results["Success"] = value;
        }
    }

    
    public decimal Value
    {
        get
        {
            if (this.Results.TryGetValue<decimal>("Value", out var value))
            {
                return value;
            }
            else
            {
                return default(decimal);
            }
        }
        set
        {
            this.Results["Value"] = value;
        }
    }

    
    public string Message
    {
        get
        {
            if (this.Results.TryGetValue<string>("Message", out var value))
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
            this.Results["Message"] = value;
        }
    }
}
