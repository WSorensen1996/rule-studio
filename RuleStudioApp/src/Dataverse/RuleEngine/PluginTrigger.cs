namespace RuleStudio.Engine;

/// <summary>The rule trigger of a plug-in execution: json-formats.md event and stage names.</summary>
public sealed class PluginTrigger
{
    private PluginTrigger(string eventName, string stage)
    {
        Event = eventName;
        Stage = stage;
    }

    // create, update or delete.
    public string Event { get; }

    // beforeSave or afterSave.
    public string Stage { get; }

    public bool BeforeSave => string.Equals(Stage, "beforeSave", StringComparison.Ordinal);

    // The trigger of a message and pipeline stage; null when rules never run there.
    public static PluginTrigger? From(string? messageName, int stage)
    {
        var eventName = messageName switch
        {
            "Create" => "create",
            "Update" => "update",
            "Delete" => "delete",
            _ => null,
        };
        if (eventName == null)
            return null;

        return stage switch
        {
            20 => new PluginTrigger(eventName, "beforeSave"),
            40 => new PluginTrigger(eventName, "afterSave"),
            _ => null,
        };
    }
}
