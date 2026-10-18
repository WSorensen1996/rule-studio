using SharedDomain.RuleStudio.Formats;
using static Tests.RuleStudio.EngineHarness;

namespace Tests.RuleStudio;

/// <summary>A chain of rules on one save: order, stop processing, block and the failure policies.</summary>
public class ChainPolicyTests
{
    private const string Before = "beforeSave";
    private const string After = "afterSave";

    [Fact]
    public void SamePriorityRunsByRuleKeyIgnoringCase()
    {
        var result = EvaluateChain(Record(), Before, Rule("b.rule", 10, Group("and")), Rule("A.RULE", 10, Group("and")), Rule("c", 5, Group("and")));
        result.Traces.Select(t => t.Rule!.RuleKey).Should().Equal("c", "A.RULE", "b.rule");
        result.Traces.Select(t => t.Sequence).Should().Equal(1, 2, 3);
    }

    [Fact]
    public void LaterRuleSeesEarlierChangeAndLastWriteWins()
    {
        var first = Rule("a", 1, Group("and"), SetField("name", "set by a"));
        var second = Rule("b", 2, Condition("name", "equals", "set by a"), SetField("description", "b saw it"), SetField("name", "set by b"));
        var result = EvaluateChain(Record(("name", "start")), Before, first, second);
        result.Changes["description"].Should().Be("b saw it");
        result.Changes["name"].Should().Be("set by b");
    }

    [Fact]
    public void StopProcessingOnlyAppliesWhenTheRuleMatches()
    {
        var stopper = Rule("a", 1, Condition("name", "equals", "stop"));
        stopper.Runtime.StopProcessing = true;
        var later = Rule("b", 2, Group("and"), SetField("description", "ran"));

        EvaluateChain(Record(("name", "go")), Before, stopper, later).Changes.Should().ContainKey("description");
        var stopped = EvaluateChain(Record(("name", "stop")), Before, stopper, later);
        stopped.Traces[1].Outcome.Should().Be("skipped");
        stopped.Traces[1].SkipReason.Should().Be("stoppedByEarlierRule");
    }

    [Fact]
    public void BlockPolicyFailureBlocksTheSaveAndSkipsLaterRules()
    {
        var failing = Rule("a", 1, Group("and"), FailingAction());
        failing.Runtime.FailurePolicy = "block";
        var result = EvaluateChain(Record(), Before, failing, Rule("b", 2, Group("and"), SetField("name", "x")));
        result.Traces[0].Outcome.Should().Be("failed");
        result.BlockMessage.Should().StartWith("a: ");
        result.Traces[1].Outcome.Should().Be("skipped");
        result.Changes.Should().BeEmpty();
    }

    [Fact]
    public void LogAndContinueFailureLetsLaterRulesRunAndDropsTheChangesOfTheFailedRule()
    {
        var failing = Rule("a", 1, Group("and"), SetField("name", "half done"), FailingAction());
        var result = EvaluateChain(Record(), Before, failing, Rule("b", 2, Group("and"), SetField("description", "ran")));
        result.Traces[0].Outcome.Should().Be("failed");
        result.Traces[0].Error!.Code.Should().Be("EXPRESSION_FAILED");
        result.BlockMessage.Should().BeNull();
        result.Changes.Should().NotContainKey("name", "the changes of a failed rule are not applied");
        result.Changes["description"].Should().Be("ran");
    }

    [Fact]
    public void AfterSaveFailureNeverBlocksEvenUnderTheBlockPolicy()
    {
        var failing = Rule("a", 1, Group("and"), FailingAction());
        failing.Runtime.FailurePolicy = "block";
        var result = EvaluateChain(Record(), After, failing, Rule("b", 2, Group("and"), SetField("description", "ran")));
        result.BlockMessage.Should().BeNull();
        result.Changes["description"].Should().Be("ran");
    }

    [Fact]
    public void BlockSaveAfterSaveFailsTheRule()
    {
        var blocker = Rule("a", 1, Group("and"), new CompiledAction { Id = Guid.NewGuid(), Type = "blockSave", Message = "No." });
        var trace = EvaluateChain(Record(), After, blocker).Traces[0];
        trace.Outcome.Should().Be("failed");
        trace.Error!.Code.Should().Be("BLOCK_SAVE_AFTER_SAVE");
    }

    [Fact]
    public void BlockSaveWithoutAMessageNamesTheRule()
    {
        var blocker = Rule("a", 1, Group("and"), new CompiledAction { Id = Guid.NewGuid(), Type = "blockSave" });
        EvaluateChain(Record(), Before, blocker).BlockMessage.Should().Be("Blocked by rule a.");
    }

    [Fact]
    public void ActionsAfterABlockAreSkipped()
    {
        var blocker = Rule("a", 1, Group("and"), new CompiledAction { Id = Guid.NewGuid(), Type = "blockSave", Message = "No." }, SetField("name", "x"));
        var trace = EvaluateChain(Record(), Before, blocker).Traces[0];
        trace.Outcome.Should().Be("blocked");
        trace.Actions.Select(a => a.Status).Should().Equal("done", "skipped");
    }

    [Theory]
    [InlineData(Before, 5, 3)]
    [InlineData(Before, 1, 2)]
    [InlineData(Before, 0, 1)]
    [InlineData(After, 5, 6)]
    [InlineData(After, 50, 11)]
    public void RetriesAreCappedPerStage(string stage, int retries, int attempts)
    {
        var failing = Rule("a", 1, Group("and"), FailingAction());
        failing.Runtime.RetryCount = retries;
        failing.Runtime.RetryDelayMs = 0;
        var result = EvaluateChain(Record(), stage, failing);
        result.Attempts[failing.RuleId].Should().Be(attempts);
        result.Traces[0].Error!.Attempt.Should().Be(attempts);
    }

    [Fact]
    public void UnknownActionTypeFails() =>
        EvaluateChain(Record(), Before, Rule("a", 1, Group("and"), new CompiledAction { Id = Guid.NewGuid(), Type = "sendEmail" })).Traces[0].Error!.Code.Should().Be("OPERATOR_NOT_SUPPORTED");

    [Fact]
    public void ConditionWithoutAFieldFails()
    {
        var node = Condition("name", "equals", "x");
        node.Path = null;
        EvaluateChain(Record(), Before, Rule("a", 1, node)).Traces[0].Error!.Code.Should().Be("FIELD_NOT_FOUND");
    }

    [Fact]
    public void SetFieldOnAColumnThatNoLongerExistsFails() =>
        EvaluateChain(Record(), Before, Rule("a", 1, Group("and"), SetField("gone", "x"))).Traces[0].Error!.Message.Should().Be("account.gone no longer exists.");
}
