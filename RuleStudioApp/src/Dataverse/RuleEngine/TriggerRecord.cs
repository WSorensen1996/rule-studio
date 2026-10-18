using Microsoft.Xrm.Sdk;

namespace RuleStudio.Engine;

/// <summary>The triggering record as the rules see it: the pre-image merged with the columns being saved.</summary>
public sealed class TriggerRecord
{
    public TriggerRecord(Entity record, Entity? preImage, ISet<string>? changedColumns)
    {
        Record = record;
        PreImage = preImage;
        ChangedColumns = changedColumns;
    }

    public Entity Record { get; }

    // Values before the save (Update and Delete), for the changed operator.
    public Entity? PreImage { get; }

    // Update: the columns in the Target. Null for Create and Delete.
    public ISet<string>? ChangedColumns { get; }

    // Builds the record from the Target (an Entity, or an EntityReference on Delete) and the pre-image.
    public static TriggerRecord Build(string table, Guid id, object? target, Entity? preImage, string eventName)
    {
        var record = new Entity(table, id);
        if (preImage != null)
        {
            foreach (var attribute in preImage.Attributes)
            {
                record[attribute.Key] = attribute.Value;
            }
        }

        var entity = target as Entity;
        if (entity != null)
        {
            foreach (var attribute in entity.Attributes)
            {
                record[attribute.Key] = attribute.Value;
            }
        }

        var changed = string.Equals(eventName, "update", StringComparison.Ordinal) && entity != null
            ? new HashSet<string>(entity.Attributes.Keys, StringComparer.OrdinalIgnoreCase)
            : null;
        return new TriggerRecord(record, string.Equals(eventName, "create", StringComparison.Ordinal) ? null : preImage, changed);
    }
}
