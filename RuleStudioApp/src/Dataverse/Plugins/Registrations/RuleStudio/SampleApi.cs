using DataverseLogic;
using DataverseLogic.RuleStudio.Samples;

namespace DataverseRegistration.RuleStudio;

/// <summary>Maps the generated Custom API requests and responses onto <see cref="SampleFunctions"/>.</summary>
public class SampleApi(SampleFunctions samples)
{
    private readonly SampleFunctions samples = samples;

#pragma warning disable CA1822 // XrmPluginCore calls Custom API handlers on the DI instance.
    public SampleCalculateDiscountResponse CalculateDiscount(SampleCalculateDiscountRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        var (success, value, message) = SampleFunctions.CalculateDiscount(request.Revenue, request.Employees);
        return new SampleCalculateDiscountResponse(success, value, message);
    }
#pragma warning restore CA1822

    public SampleStampContactsResponse StampContacts(SampleStampContactsRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        var (success, value, message) = samples.StampContacts(request.AccountId, request.Text);
        return new SampleStampContactsResponse(success, value, message);
    }
}
