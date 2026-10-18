namespace DataverseLogic.RuleStudio.Steps;

/// <summary>An existing engine step that must be changed to match a desired one.</summary>
public sealed class StepChange
{
    public StepChange(ExistingStep existing, DesiredStep desired)
    {
        Existing = existing;
        Desired = desired;
    }

    public ExistingStep Existing { get; }

    public DesiredStep Desired { get; }

    // The step row itself differs: name, filtering attributes or async auto-delete.
    public bool UpdateStep { get; set; }

    // The pre-image is missing.
    public bool AddPreImage { get; set; }
}
