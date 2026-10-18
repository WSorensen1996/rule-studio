namespace DataverseLogic.RuleStudio.Engine;

/// <summary>Guards that keep one rule inside the plug-in time limit.</summary>
public static class EngineLimits
{
    // Hops in one field path; publishing checks the same number.
    public const int MaxHops = 5;

    // Child rows read for Any / All / None-of, and lookup rows matched, per evaluation. Queries ask
    // for one row more to detect the limit, and Dataverse allows at most 5000 rows per query.
    public const int MaxRows = 4999;

    // Before-save rules run inside the user's save, so their retries are capped.
    public const int MaxBeforeSaveRetries = 2;

    public const int MaxBeforeSaveRetryDelayMs = 1000;

    public const int MaxAfterSaveRetries = 10;

    public const int MaxAfterSaveRetryDelayMs = 60000;
}
