namespace DataverseLogic.RuleStudio.Steps;

/// <summary>An sdkmessageprocessingstep of the engine plug-in type, as registered now.</summary>
public sealed class ExistingStep
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Table { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public int Stage { get; set; }

    public int Mode { get; set; }

    // As stored: may be unsorted or null.
    public string? FilteringAttributes { get; set; }

    public bool AsyncAutoDelete { get; set; }

    // The step has the engine's PreImage registered.
    public bool HasPreImage { get; set; }

    public string Key => StepPlanner.Key(Table, Message, Stage, Mode);
}
