extern alias engine;

using engine::RuleStudio.Engine;
using Microsoft.Xrm.Sdk;
using RuleStudio.SharedContext;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace Tests.RuleStudio;

/// <summary>
/// Which runs get an rs_executionlog row, how much of the trace each level keeps, and the row's
/// columns (json-formats.md §4).
/// </summary>
public class LoggingTests
{
    private const string Table = "account";

    private static readonly string[] Outcomes = ["matched", "notMatched", "skipped", "failed", "blocked"];

    // Before save: Off logs nothing, Errors only logs failed and blocked runs, Summary and Full trace log every run.
    // After save: the same, except that a failure is always logged (it needs an admin).
    public static TheoryData<string, string, int, bool> Matrix()
    {
        var data = new TheoryData<string, string, int, bool>();
        foreach (var outcome in Outcomes)
        {
            var error = outcome is "failed" or "blocked";
            foreach (var stage in new[] { 20, 40 })
            {
                data.Add("off", outcome, stage, stage == 40 && error);
                data.Add("errorsOnly", outcome, stage, error);
                data.Add("summary", outcome, stage, true);
                data.Add("fullTrace", outcome, stage, true);
            }
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(Matrix))]
    public void LogLevelOutcomeAndStageDecideWhetherARowIsWritten(string level, string outcome, int stage, bool logged) =>
        LogPolicy.ShouldLog(Run(level, outcome), Trigger(stage)).Should().Be(logged);

    [Fact]
    public void AfterSaveApplyFailureIsAnErrorEvenWhenTheRuleMatched()
    {
        var run = Run("off", "matched");
        run.ApplyError = "No write privilege.";
        LogPolicy.ShouldLog(run, Trigger(40)).Should().BeTrue();
        LogPolicy.NeedsAdmin(run, Trigger(40)).Should().BeTrue();
        LogPolicy.IsError(run).Should().BeTrue();
    }

    [Fact]
    public void BeforeSaveFailuresNeverNeedAnAdmin() =>
        LogPolicy.NeedsAdmin(Run("errorsOnly", "failed"), Trigger(20)).Should().BeFalse();

    [Fact]
    public void FullTraceKeepsEverything()
    {
        var run = Detailed("fullTrace");
        LogPolicy.Shape(run);
        run.Trace.Level.Should().Be("fullTrace");
        run.Trace.Conditions.Should().NotBeNull();
        run.Trace.Branch.Should().Be("then");
        run.Trace.Actions.Should().ContainSingle();
    }

    [Theory]
    [InlineData("summary")]
    [InlineData("errorsOnly")]
    [InlineData("off")]
    public void OtherLevelsKeepOnlyTheHeaderAndTheError(string level)
    {
        var run = Detailed(level);
        run.Trace.Error = new TraceError { Code = "RULE_FAILED", Message = "x", Attempt = 1 };
        LogPolicy.Shape(run);
        run.Trace.Level.Should().Be("summary");
        run.Trace.Conditions.Should().BeNull();
        run.Trace.Branch.Should().BeNull();
        run.Trace.Actions.Should().BeEmpty();
        run.Trace.Error!.Code.Should().Be("RULE_FAILED");
    }

    [Fact]
    public void ApplyErrorBecomesTheTraceError()
    {
        var run = Run("summary", "failed");
        run.ApplyError = "The changes of the rule could not be saved: denied";
        LogPolicy.Shape(run);
        run.Trace.Error!.Code.Should().Be("APPLY_FAILED");
        run.Trace.Error.Message.Should().Be(run.ApplyError);
    }

    [Theory]
    [InlineData("matched", rs_executionlog_rs_outcome.Matched)]
    [InlineData("notMatched", rs_executionlog_rs_outcome.Notmatched)]
    [InlineData("skipped", rs_executionlog_rs_outcome.Skipped)]
    [InlineData("failed", rs_executionlog_rs_outcome.Failed)]
    [InlineData("blocked", rs_executionlog_rs_outcome.Blocked)]
    public void RowOutcomeMapsTheTraceOutcome(string outcome, rs_executionlog_rs_outcome expected) =>
        ExecutionLogWriter.ToLog(Run("fullTrace", outcome), Trigger(20), Table, Guid.Empty).rs_Outcome.Should().Be(expected);

    [Theory]
    [InlineData("Create", 20, rs_event.Create, rs_stage.Beforesave)]
    [InlineData("Update", 40, rs_event.Update, rs_stage.Aftersave)]
    [InlineData("Delete", 20, rs_event.Delete, rs_stage.Beforesave)]
    public void RowEventAndStageMapTheTrigger(string message, int stage, rs_event eventChoice, rs_stage stageChoice)
    {
        var log = ExecutionLogWriter.ToLog(Run("fullTrace", "matched"), PluginTrigger.From(message, stage)!, Table, Guid.Empty);
        log.rs_Event.Should().Be(eventChoice);
        log.rs_Stage.Should().Be(stageChoice);
    }

    [Fact]
    public void RowCarriesTheRuleRecordAndCorrelation()
    {
        var run = Run("errorsOnly", "failed");
        run.Trace.Error = new TraceError { Code = "RULE_FAILED", Message = new string('e', 5000), Attempt = 2 };
        run.Trace.DurationMs = 42;
        var correlation = Guid.NewGuid();

        var log = ExecutionLogWriter.ToLog(run, Trigger(20), Table, correlation);

        log.rs_RuleId!.Id.Should().Be(run.Rule.Rule.RuleId);
        log.rs_RuleVersionId!.Id.Should().Be(run.Rule.Rule.RuleVersionId);
        log.rs_RecordId.Should().Be(run.Trace.Record.Id.ToString());
        log.rs_TargetTable.Should().Be(Table);
        log.rs_CorrelationId.Should().Be(correlation.ToString());
        log.rs_Name.Should().Be("r · failed");
        log.rs_DurationMs.Should().Be(42);
        log.rs_Attempts.Should().Be(1);
        log.rs_ErrorMessage!.Length.Should().Be(4000);
        log.rs_ErrorMessage.Should().StartWith("RULE_FAILED: ").And.EndWith("…");
        log.rs_LogLevel.Should().Be(rs_loglevel.Errorsonly);
        log.rs_IsTest.Should().BeFalse();
    }

    [Fact]
    public void UnsavedRecordHasNoRecordId()
    {
        var run = Run("fullTrace", "matched");
        run.Trace.Record.Id = Guid.Empty;
        ExecutionLogWriter.ToLog(run, Trigger(20), Table, Guid.Empty).rs_RecordId.Should().BeNull();
    }

    [Fact]
    public void RuleWithoutALevelIsLoggedAsErrorsOnly() =>
        ExecutionLogWriter.ToLog(Run(null, "failed"), Trigger(20), Table, Guid.Empty).rs_LogLevel.Should().Be(rs_loglevel.Errorsonly);

    [Fact]
    public void WriterSkipsRunsTheLevelLeavesOutAndSurvivesAFailingWrite()
    {
        var outcome = new ChainOutcome();
        outcome.Runs.Add(Run("off", "matched"));
        outcome.Runs.Add(Run("fullTrace", "matched"));
        outcome.Runs.Add(Run("fullTrace", "notMatched"));
        var dao = Substitute.For<IDataverseAccessObject>();
        var calls = 0;
        dao.Create(Arg.Any<Entity>(), Arg.Any<string>()).Returns(_ => ++calls == 1 ? throw new InvalidOperationException("storage full") : Guid.NewGuid());
        var tracing = Substitute.For<ITracingService>();

        var written = new ExecutionLogWriter(dao, tracing).Write(outcome, Trigger(20), Table, Guid.NewGuid());

        written.Should().Be(1);
        dao.Received(2).Create(Arg.Any<Entity>(), Arg.Any<string>());
        tracing.Received(1).Trace(Arg.Is<string>(s => s.Contains("could not write the execution log", StringComparison.Ordinal)), Arg.Any<object[]>());
    }

    private static PluginTrigger Trigger(int stage) => PluginTrigger.From("Update", stage)!;

    private static RuleRunRecord Run(string? level, string outcome)
    {
        var rule = new CompiledRule { RuleId = Guid.NewGuid(), RuleKey = "r", RuleVersionId = Guid.NewGuid(), Conditions = new CompiledNode { Type = "group", Operator = "and" } };
        rule.Runtime.LogLevel = level;
        var trace = new ExecutionTrace { Level = "fullTrace", Outcome = outcome, Record = new RecordReference { Table = Table, Id = Guid.NewGuid() } };
        return new RuleRunRecord(new LiveRule(rule), trace, 1);
    }

    private static RuleRunRecord Detailed(string level)
    {
        var run = Run(level, "matched");
        run.Trace.Conditions = new TraceNode { Type = "group", Operator = "and", Result = true };
        run.Trace.Branch = "then";
        run.Trace.Actions.Add(new ActionTrace { Type = "setField", Status = "done" });
        return run;
    }
}
