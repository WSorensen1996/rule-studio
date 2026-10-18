using DataverseLogic.RuleStudio.Steps;
using SharedDomain.RuleStudio.Formats;

namespace Tests.RuleStudio;

/// <summary>The pure part of engine step reconciliation: desired steps and the plan against registered ones.</summary>
public class StepPlannerTests
{
    [Fact]
    public void OneStepPerTableMessageAndStage()
    {
        var steps = StepPlanner.Desired(
        [
            Trigger("Account", "update", "beforeSave", "name"),
            Trigger("account", "update", "beforeSave", "revenue", "Name"),
            Trigger("account", "update", "afterSave"),
            Trigger("account", "create", "beforeSave"),
            Trigger("contact", "delete", "afterSave"),
        ]);

        steps.Should().HaveCount(4);
        var preUpdate = steps.Single(s => s.Message == "Update" && s.Stage == StepPlanner.PreOperation);
        preUpdate.Table.Should().Be("account");
        preUpdate.FilteringAttributes.Should().Be("name,revenue");
        preUpdate.Mode.Should().Be(StepPlanner.Synchronous);
        preUpdate.PreImage.Should().BeTrue();
        preUpdate.Name.Should().Be("Rule Studio: account Update PreOperation");

        var postUpdate = steps.Single(s => s.Message == "Update" && s.Stage == StepPlanner.PostOperation);
        postUpdate.Mode.Should().Be(StepPlanner.Asynchronous);
        postUpdate.FilteringAttributes.Should().BeEmpty();

        steps.Single(s => s.Message == "Create").PreImage.Should().BeFalse();
        steps.Single(s => s.Message == "Delete").PreImage.Should().BeTrue();
    }

    [Fact]
    public void AnyUnfilteredUpdateRuleRemovesTheFilter()
    {
        var steps = StepPlanner.Desired([Trigger("account", "update", "beforeSave", "name"), Trigger("account", "update", "beforeSave")]);
        steps.Single().FilteringAttributes.Should().BeEmpty();
    }

    [Fact]
    public void IncompleteTriggersNeedNoStep()
    {
        StepPlanner.Desired([Trigger(string.Empty, "update", "beforeSave"), Trigger("account", string.Empty, "beforeSave"), Trigger("account", "update", string.Empty)])
            .Should().BeEmpty();
    }

    [Fact]
    public void PlanCreatesMissingAndKeepsMatchingSteps()
    {
        var desired = StepPlanner.Desired([Trigger("account", "update", "beforeSave", "name"), Trigger("account", "create", "afterSave")]);
        var matching = Existing(desired.Single(s => s.Message == "Update"));

        var plan = StepPlanner.Plan(desired, [matching]);

        plan.Create.Should().ContainSingle().Which.Message.Should().Be("Create");
        plan.Unchanged.Should().Be(1);
        plan.Change.Should().BeEmpty();
        plan.Delete.Should().BeEmpty();
    }

    [Fact]
    public void PlanUpdatesFilterAutoDeleteAndMissingImage()
    {
        var desired = StepPlanner.Desired([Trigger("account", "update", "beforeSave", "name", "revenue"), Trigger("account", "update", "afterSave")]);
        var pre = Existing(desired.Single(s => s.Stage == StepPlanner.PreOperation));
        pre.FilteringAttributes = "revenue , NAME";
        pre.HasPreImage = false;
        var post = Existing(desired.Single(s => s.Stage == StepPlanner.PostOperation));
        post.AsyncAutoDelete = false;

        var plan = StepPlanner.Plan(desired, [pre, post]);

        var preChange = plan.Change.Single(c => c.Existing == pre);
        preChange.UpdateStep.Should().BeFalse("the stored filter is the same set of columns");
        preChange.AddPreImage.Should().BeTrue();
        plan.Change.Single(c => c.Existing == post).UpdateStep.Should().BeTrue();
    }

    [Fact]
    public void PlanDeletesUnusedDuplicateAndWrongModeSteps()
    {
        var desired = StepPlanner.Desired([Trigger("account", "update", "afterSave")]);
        var kept = Existing(desired[0]);
        var duplicate = Existing(desired[0]);
        var unused = Existing(StepPlanner.Desired([Trigger("contact", "create", "beforeSave")])[0]);
        var wrongMode = Existing(desired[0]);
        wrongMode.Mode = StepPlanner.Synchronous;

        var plan = StepPlanner.Plan(desired, [kept, duplicate, unused, wrongMode]);

        plan.Delete.Should().BeEquivalentTo([duplicate, unused, wrongMode]);
        plan.Unchanged.Should().Be(1);
        plan.Create.Should().BeEmpty();
    }

    [Fact]
    public void NoPublishedRulesRemoveEveryStep()
    {
        var existing = Existing(StepPlanner.Desired([Trigger("account", "update", "afterSave")])[0]);
        StepPlanner.Plan([], [existing]).Delete.Should().ContainSingle();
    }

    private static RuleTrigger Trigger(string table, string eventName, string stage, params string[] filtered)
    {
        var trigger = new RuleTrigger { Table = table, Event = eventName, Stage = stage };
        foreach (var column in filtered)
        {
            trigger.FilteredColumns.Add(column);
        }

        return trigger;
    }

    private static ExistingStep Existing(DesiredStep step) => new()
    {
        Id = Guid.NewGuid(),
        Name = step.Name,
        Table = step.Table,
        Message = step.Message,
        Stage = step.Stage,
        Mode = step.Mode,
        FilteringAttributes = step.FilteringAttributes.Length == 0 ? null : step.FilteringAttributes,
        AsyncAutoDelete = step.Mode == StepPlanner.Asynchronous,
        HasPreImage = step.PreImage,
    };
}
