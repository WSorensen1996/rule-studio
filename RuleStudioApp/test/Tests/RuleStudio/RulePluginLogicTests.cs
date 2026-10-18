extern alias engine;

using engine::RuleStudio.Engine;
using Microsoft.Xrm.Sdk;
using RuleStudio.SharedContext;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;
using IFunctionInvoker = engine::DataverseLogic.RuleStudio.Engine.IFunctionInvoker;

namespace Tests.RuleStudio;

/// <summary>The engine plug-in's own logic (src/Dataverse/RuleEngine) without Dataverse: selection, run-as batches, apply and logging.</summary>
public class RulePluginLogicTests
{
    private const string Table = "account";

    [Theory]
    [InlineData("Create", 20, "create", "beforeSave")]
    [InlineData("Update", 40, "update", "afterSave")]
    [InlineData("Delete", 20, "delete", "beforeSave")]
    public void TriggerFromMessageAndStage(string message, int stage, string eventName, string stageName)
    {
        var trigger = PluginTrigger.From(message, stage)!;
        trigger.Event.Should().Be(eventName);
        trigger.Stage.Should().Be(stageName);
    }

    [Theory]
    [InlineData("Retrieve", 20)]
    [InlineData("Update", 10)]
    [InlineData("Update", 30)]
    public void NoTriggerForOtherMessagesAndStages(string message, int stage) =>
        PluginTrigger.From(message, stage).Should().BeNull();

    [Fact]
    public void RecordMergesTargetOverPreImage()
    {
        var id = Guid.NewGuid();
        var pre = new Entity(Table, id) { ["name"] = "Old", ["revenue"] = new Money(5) };
        var target = new Entity(Table, id) { ["name"] = "New" };

        var record = TriggerRecord.Build(Table, id, target, pre, "update");

        record.Record["name"].Should().Be("New");
        record.Record.GetAttributeValue<Money>("revenue").Value.Should().Be(5);
        record.ChangedColumns.Should().BeEquivalentTo("name");
        record.PreImage.Should().BeSameAs(pre);
    }

    [Fact]
    public void DeleteRecordComesFromThePreImage()
    {
        var id = Guid.NewGuid();
        var record = TriggerRecord.Build(Table, id, new EntityReference(Table, id), new Entity(Table, id) { ["name"] = "Gone" }, "delete");
        record.Record["name"].Should().Be("Gone");
        record.ChangedColumns.Should().BeNull();
    }

    [Fact]
    public void SelectionSkipsUnchangedFilteredColumnsAndOrdersByPriorityThenKey()
    {
        var trigger = PluginTrigger.From("Update", 20)!;
        var filtered = Rule("filtered", 1);
        filtered.Rule.Trigger.FilteredColumns.Add("revenue");
        var b = Rule("b", 5);
        var a = Rule("A", 5);
        var first = Rule("z", 1);
        var otherStage = Rule("other", 1, stage: "afterSave");

        var selected = RuleSelection.Applicable([filtered, b, a, first, otherStage], Table, trigger, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "name" });

        selected.Select(r => r.Rule.RuleKey).Should().Equal("z", "A", "b");
    }

    [Fact]
    public void RulesRunWithTheirOwnIdentityAndSeeEarlierChanges()
    {
        var user = Substitute.For<IDataverseAccessObject>();
        var system = Substitute.For<IDataverseAccessObject>();
        var used = new List<bool>();
        var runner = new ChainRunner(
            asSystem =>
            {
                used.Add(asSystem);
                return asSystem ? system : user;
            },
            _ => Substitute.For<IFunctionInvoker>());
        var first = Rule("first", 1, SetField("name", "from first"));
        var second = Rule("second", 2, SetField("description", "second ran"), runAs: "system");
        second.Rule.Conditions = Condition("name", "equals", "from first");
        var third = Rule("third", 3, SetField("name", "from third"));

        var outcome = runner.Run(Record(), PluginTrigger.From("Update", 20)!, [first, second, third]);

        used.Should().Equal(false, true, false);
        outcome.Runs.Select(r => (r.Trace.Sequence, r.Trace.Outcome)).Should().Equal((1, "matched"), (2, "matched"), (3, "matched"));
        outcome.Changes["name"].Value.Should().Be("from third");
        outcome.Changes["name"].AsSystem.Should().BeFalse();
        outcome.Changes["description"].AsSystem.Should().BeTrue();
        outcome.Runs[0].Attempts.Should().Be(1);
    }

    [Fact]
    public void StopProcessingAndBlockSkipLaterBatches()
    {
        var runner = new ChainRunner(_ => Substitute.For<IDataverseAccessObject>(), _ => Substitute.For<IFunctionInvoker>());
        var stopper = Rule("stop", 1);
        stopper.Rule.Runtime.StopProcessing = true;
        var later = Rule("later", 2, SetField("name", "x"), runAs: "system");

        var stopped = runner.Run(Record(), PluginTrigger.From("Update", 20)!, [stopper, later]);
        stopped.Runs[1].Trace.Outcome.Should().Be("skipped");
        stopped.Runs[1].Trace.SkipReason.Should().Be("stoppedByEarlierRule");
        stopped.Changes.Should().BeEmpty();

        var blocker = Rule("block", 1, new CompiledAction { Id = Guid.NewGuid(), Type = "blockSave", Message = "No." });
        var blocked = runner.Run(Record(), PluginTrigger.From("Update", 20)!, [blocker, Rule("after", 2, runAs: "system")]);
        blocked.BlockMessage.Should().Be("No.");
        blocked.Runs[1].Trace.Outcome.Should().Be("skipped");
    }

    [Fact]
    public void BeforeSaveChangesGoIntoTheTarget()
    {
        var outcome = new ChainOutcome();
        outcome.Changes["name"] = new FieldChange("set", asSystem: true);
        var target = new Entity(Table, Guid.NewGuid()) { ["name"] = "typed" };

        ChangeApplier.ApplyToTarget(target, outcome);

        target["name"].Should().Be("set");
    }

    [Fact]
    public void AfterSaveUpdatesOncePerIdentityAndReportsFailures()
    {
        var outcome = new ChainOutcome();
        outcome.Changes["name"] = new FieldChange("a", asSystem: true);
        outcome.Changes["description"] = new FieldChange("b", asSystem: false);
        var system = Substitute.For<IDataverseAccessObject>();
        var user = Substitute.For<IDataverseAccessObject>();
        user.When(d => d.Update(Arg.Any<Entity>(), Arg.Any<string>())).Do(_ => throw new InvalidPluginExecutionException("No write privilege."));

        var error = ChangeApplier.UpdateRecord(Table, Guid.NewGuid(), outcome, asSystem => asSystem ? system : user);

        system.Received(1).Update(Arg.Is<Entity>(e => e.Attributes.Count == 1 && (string)e["name"] == "a"), Arg.Any<string>());
        error.Should().Be("No write privilege.");
    }

    [Theory]
    [InlineData("off", "matched", "beforeSave", false)]
    [InlineData("errorsOnly", "matched", "beforeSave", false)]
    [InlineData("errorsOnly", "failed", "beforeSave", true)]
    [InlineData("summary", "notMatched", "beforeSave", true)]
    [InlineData("fullTrace", "skipped", "afterSave", true)]
    [InlineData("off", "failed", "afterSave", true)]
    public void LogLevelDecidesWhichRunsAreLogged(string level, string outcome, string stage, bool logged)
    {
        var run = Run(Rule("r", 1, logLevel: level), outcome);
        LogPolicy.ShouldLog(run, PluginTrigger.From("Update", stage == "beforeSave" ? 20 : 40)!).Should().Be(logged);
    }

    [Fact]
    public void RuleLevelAppliesElseErrorsOnly()
    {
        Rule("r", 1).LogLevel.Should().Be("errorsOnly");
        Rule("r", 1, logLevel: "off").LogLevel.Should().Be("off");
    }

    [Fact]
    public void SummaryLogKeepsTheHeaderAndAfterSaveFailuresAreFlagged()
    {
        var run = Run(Rule("r", 1, logLevel: "summary"), "failed");
        run.Trace.Branch = "then";
        run.Trace.Actions.Add(new ActionTrace { Type = "setField", Status = "failed" });
        run.Trace.Error = new TraceError { Code = "FUNCTION_FAILED", Message = "Timeout", Attempt = 2 };

        var log = ExecutionLogWriter.ToLog(run, PluginTrigger.From("Update", 40)!, Table, Guid.Empty);

        run.Trace.Level.Should().Be("summary");
        run.Trace.Actions.Should().BeEmpty();
        log.rs_Outcome.Should().Be(rs_executionlog_rs_outcome.Failed);
        log.rs_Stage.Should().Be(rs_stage.Aftersave);
        log.rs_AdminFlag.Should().BeTrue();
        log.rs_IsTest.Should().BeFalse();
        log.rs_ErrorMessage.Should().Be("FUNCTION_FAILED: Timeout");
        log.rs_LogLevel.Should().Be(rs_loglevel.Summary);
        log.rs_Trace.Should().Contain("\"level\":\"summary\"").And.Contain("\"actions\":[]");
    }

    [Fact]
    public void LongTracesAreCutFromTheEnd()
    {
        var trace = new ExecutionTrace { Level = "fullTrace", Outcome = "matched" };
        for (var i = 0; i < 50; i++)
        {
            trace.Actions.Add(new ActionTrace { Type = "logMessage", Status = "done", Message = new string('x', 100) });
        }

        var text = TraceJson.Write(trace, 2000);

        text.Length.Should().BeLessThanOrEqualTo(2000);
        trace.Truncated.Should().BeTrue();
        trace.Actions.Should().NotBeEmpty();
    }

    private static TriggerRecord Record() => TriggerRecord.Build(Table, Guid.NewGuid(), new Entity(Table) { ["name"] = "start" }, null, "update");

    private static RuleRunRecord Run(LiveRule rule, string outcome) =>
        new(rule, new ExecutionTrace { Level = "fullTrace", Outcome = outcome, Record = new RecordReference { Table = Table, Id = Guid.NewGuid() } }, 1);

    private static LiveRule Rule(string key, int priority, CompiledAction? action = null, string runAs = "callingUser", string stage = "beforeSave", string? logLevel = null)
    {
        var rule = new CompiledRule { RuleId = Guid.NewGuid(), RuleKey = key, RuleVersionId = Guid.NewGuid(), Conditions = new CompiledNode { Type = "group", Operator = "and" } };
        rule.Trigger.Table = Table;
        rule.Trigger.Event = "update";
        rule.Trigger.Stage = stage;
        rule.Runtime.Priority = priority;
        rule.Runtime.RunAs = runAs;
        rule.Runtime.FailurePolicy = "logAndContinue";
        rule.Runtime.LogLevel = logLevel;
        if (action != null)
        {
            rule.Then.Add(action);
        }

        return new LiveRule(rule);
    }

    private static CompiledAction SetField(string column, string value) =>
        new() { Id = Guid.NewGuid(), Type = "setField", TargetField = column, Value = new ValueSource { Type = "fixed", Value = value } };

    private static CompiledNode Condition(string column, string op, object value)
    {
        var group = new CompiledNode { Type = "group", Operator = "and" };
        group.Children.Add(new CompiledNode
        {
            Type = "condition",
            Id = Guid.NewGuid(),
            Path = new FieldPath { Table = Table, Column = column },
            Operator = op,
            Value = new ValueSource { Type = "fixed", Value = value },
        });
        return group;
    }
}
