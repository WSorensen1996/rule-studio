namespace RuleStudio.Engine;

/// <summary>A column a rule set, and whether that rule runs as the system user.</summary>
public sealed class FieldChange
{
    public FieldChange(object? value, bool asSystem)
    {
        Value = value;
        AsSystem = asSystem;
    }

    public object? Value { get; }

    public bool AsSystem { get; }
}
