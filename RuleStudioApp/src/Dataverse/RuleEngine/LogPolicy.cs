using DataverseLogic;
using SharedDomain.RuleStudio.Formats;

namespace RuleStudio.Engine;

/// <summary>
/// Decides per rule whether a production run writes an rs_executionlog row and how much of the
/// trace it keeps (json-formats.md §4). After-save failures are always logged and
/// flagged for an admin, whatever the log level.
/// </summary>
public static class LogPolicy
{
    public const string Off = "off";
    public const string ErrorsOnly = "errorsOnly";
    public const string Summary = "summary";
    public const string FullTrace = "fullTrace";

    // True when the run is a failure: failed or blocked, or its after-save update failed.
    public static bool IsError(RuleRunRecord run)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(run, nameof(run));
        return run.ApplyError != null
            || string.Equals(run.Trace.Outcome, "failed", StringComparison.Ordinal)
            || string.Equals(run.Trace.Outcome, "blocked", StringComparison.Ordinal);
    }

    // After-save failures need an admin: they cannot block the save, so nobody else sees them.
    public static bool NeedsAdmin(RuleRunRecord run, PluginTrigger trigger)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(trigger, nameof(trigger));
        return !trigger.BeforeSave && IsError(run);
    }

    // Whether the run gets a log row.
    public static bool ShouldLog(RuleRunRecord run, PluginTrigger trigger)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(run, nameof(run));
        if (NeedsAdmin(run, trigger))
            return true;

        return run.Rule.LogLevel switch
        {
            Off => false,
            Summary or FullTrace => true,
            _ => IsError(run),
        };
    }

    // Cuts the trace down to what the level keeps: everything for Full trace, the header (and the
    // error) for the others.
    public static void Shape(RuleRunRecord run)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(run, nameof(run));
        var trace = run.Trace;
        if (run.ApplyError != null)
        {
            trace.Error ??= new TraceError { Code = "APPLY_FAILED", Message = run.ApplyError, Attempt = 1 };
        }

        if (string.Equals(run.Rule.LogLevel, FullTrace, StringComparison.Ordinal))
        {
            trace.Level = FullTrace;
            return;
        }

        trace.Level = Summary;
        trace.Conditions = null;
        trace.Branch = null;
        trace.Actions.Clear();
    }
}
