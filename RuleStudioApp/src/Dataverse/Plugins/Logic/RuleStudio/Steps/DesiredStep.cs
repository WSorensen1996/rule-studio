namespace DataverseLogic.RuleStudio.Steps;

/// <summary>An engine plug-in step the published rules need.</summary>
public sealed class DesiredStep
{
    public DesiredStep(string table, string message, int stage, string filteringAttributes)
    {
        Table = table;
        Message = message;
        Stage = stage;
        FilteringAttributes = filteringAttributes;
    }

    // Logical name of the table.
    public string Table { get; }

    // Create, Update or Delete (sdkmessage name).
    public string Message { get; }

    // 20 PreOperation (before save) or 40 PostOperation (after save).
    public int Stage { get; }

    // 0 synchronous (before save) or 1 asynchronous (after save).
    public int Mode => Stage == StepPlanner.PostOperation ? StepPlanner.Asynchronous : StepPlanner.Synchronous;

    // Comma-separated, sorted, lowercase; empty means every column (Update only).
    public string FilteringAttributes { get; }

    // Update and Delete steps register the pre-image the engine reads.
    public bool PreImage => !string.Equals(Message, "Create", StringComparison.Ordinal);

    public string Name => $"Rule Studio: {Table} {Message} {(Stage == StepPlanner.PreOperation ? "PreOperation" : "PostOperation")}";

    public string Key => StepPlanner.Key(Table, Message, Stage, Mode);
}
