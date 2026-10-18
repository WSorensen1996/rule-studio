using DataverseLogic;
using Microsoft.Xrm.Sdk;
using RuleStudio.SharedContext;
using SharedContext.Dao;

namespace RuleStudio.Engine;

/// <summary>
/// Writes rs_executionlog rows for a production run (istest = false), one per rule (json-formats.md,
/// Decisions 4), as the system user so the log does not depend on the caller's privileges. A failing
/// write is traced and skipped. Limitation: inside a before-save transaction a failed write still
/// aborts the transaction, and rows written before a block are rolled back (docs/engine.md).
/// </summary>
public sealed class ExecutionLogWriter(IDataverseAccessObject dao, ITracingService tracing)
{
    private readonly IDataverseAccessObject dao = dao;
    private readonly ITracingService tracing = tracing;

    // Writes the rows the log policy asks for; returns how many were written.
    public int Write(ChainOutcome outcome, PluginTrigger trigger, string table, Guid correlationId)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(outcome, nameof(outcome));
        var written = 0;
        foreach (var run in outcome.Runs.Where(r => LogPolicy.ShouldLog(r, trigger)))
        {
            try
            {
                dao.Create(ToLog(run, trigger, table, correlationId));
                written++;
            }
#pragma warning disable CA1031 // A log row must never break the save it describes.
            catch (Exception ex)
#pragma warning restore CA1031
            {
                tracing.Trace("Rule Studio: could not write the execution log of {0}: {1}", run.Rule.Rule.RuleKey, ex.Message);
            }
        }

        return written;
    }

    // The log row of one rule run, shaped by its log level.
    public static rs_ExecutionLog ToLog(RuleRunRecord run, PluginTrigger trigger, string table, Guid correlationId)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(run, nameof(run));
        ArgumentNullExceptionHelper.ThrowIfNull(trigger, nameof(trigger));
        ArgumentNullExceptionHelper.ThrowIfNull(table, nameof(table));
        LogPolicy.Shape(run);
        var rule = run.Rule.Rule;
        var trace = run.Trace;
        return new rs_ExecutionLog
        {
            rs_Name = Cut($"{rule.RuleKey} · {trace.Outcome}", EngineSettings.MaxNameLength),
            rs_RuleId = new EntityReference(rs_Rule.EntityLogicalName, rule.RuleId),
            rs_RuleVersionId = new EntityReference(rs_RuleVersion.EntityLogicalName, rule.RuleVersionId),
            rs_TargetTable = Cut(table, EngineSettings.MaxShortTextLength),
            rs_RecordId = trace.Record.Id == Guid.Empty ? null : trace.Record.Id.ToString(),
            rs_Event = Event(trigger.Event),
            rs_Stage = trigger.BeforeSave ? rs_stage.Beforesave : rs_stage.Aftersave,
            rs_Outcome = Outcome(trace.Outcome),
            rs_Trace = TraceJson.Write(trace, EngineSettings.MaxTraceLength),
            rs_DurationMs = trace.DurationMs,
            rs_Attempts = run.Attempts,
            rs_ErrorMessage = trace.Error == null ? null : Cut($"{trace.Error.Code}: {trace.Error.Message}", EngineSettings.MaxErrorLength),
            rs_CorrelationId = correlationId.ToString(),
            rs_IsTest = false,
            rs_AdminFlag = LogPolicy.NeedsAdmin(run, trigger),
            rs_LogLevel = Level(run.Rule.LogLevel),
        };
    }

    private static string Cut(string text, int max) => text.Length <= max ? text : text.Remove(max - 1) + "…";

    private static rs_event Event(string name) => name switch
    {
        "create" => rs_event.Create,
        "delete" => rs_event.Delete,
        _ => rs_event.Update,
    };

    private static rs_executionlog_rs_outcome Outcome(string name) => name switch
    {
        "matched" => rs_executionlog_rs_outcome.Matched,
        "skipped" => rs_executionlog_rs_outcome.Skipped,
        "failed" => rs_executionlog_rs_outcome.Failed,
        "blocked" => rs_executionlog_rs_outcome.Blocked,
        _ => rs_executionlog_rs_outcome.Notmatched,
    };

    private static rs_loglevel Level(string name) => name switch
    {
        LogPolicy.Off => rs_loglevel.Off,
        LogPolicy.Summary => rs_loglevel.Summary,
        LogPolicy.FullTrace => rs_loglevel.Fulltrace,
        _ => rs_loglevel.Errorsonly,
    };
}
