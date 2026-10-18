namespace DataverseLogic.RuleStudio.Steps;

/// <summary>What the reconciler must do so the engine steps match the published rules exactly.</summary>
public sealed class StepPlan
{
    public IList<DesiredStep> Create { get; } = new List<DesiredStep>();

    public IList<StepChange> Change { get; } = new List<StepChange>();

    // Steps no published rule needs, and duplicates.
    public IList<ExistingStep> Delete { get; } = new List<ExistingStep>();

    public int Unchanged { get; set; }
}
