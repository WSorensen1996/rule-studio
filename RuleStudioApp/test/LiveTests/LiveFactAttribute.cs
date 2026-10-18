namespace LiveTests;

/// <summary>Skipped unless RULESTUDIO_LIVE_URL names the Dataverse environment to test against.</summary>
public sealed class LiveFactAttribute : FactAttribute
{
    public LiveFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(LiveEnvironment.Url))
        {
            Skip = "Set RULESTUDIO_LIVE_URL to run the live tests against a Dataverse environment.";
        }
    }
}
