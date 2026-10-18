using Microsoft.Xrm.Sdk;
using RuleStudio.SharedContext;
using SharedContext.Dao;
using CrmTask = RuleStudio.SharedContext.Task;

namespace DataverseLogic.RuleStudio.Functions;

/// <summary>
/// Standard custom functions that change records: they have side effects, so tests never run them.
/// They run as the user the rule runs as. A record is given as a table name and an id (for the
/// record the rule runs on: its table and its primary key column), an owner as a user or team lookup.
/// </summary>
public class RecordFunctions(IUserDataverseAccessObjectService dao)
{
    private const int MaxSubjectLength = 200;

    private readonly IUserDataverseAccessObjectService dao = dao;

    // Creates an open task, optionally regarding a record, due in a number of days and owned by a user or team. Not safe to retry.
    public (bool Success, EntityReference? Value, string Message) CreateTask(string? regardingTable, string? regardingId, string? subject, string? description, int? dueInDays, EntityReference? owner)
    {
        var title = subject?.Trim() ?? string.Empty;
        if (title.Length == 0)
            return (false, null, "Subject is required.");

        if (title.Length > MaxSubjectLength)
            return (false, null, $"Subject has at most {MaxSubjectLength} characters.");

        var hasTable = !string.IsNullOrWhiteSpace(regardingTable);
        var hasId = !string.IsNullOrWhiteSpace(regardingId);
        if (hasTable != hasId)
            return (false, null, "Give both RegardingTable and RegardingId, or neither.");

        EntityReference? regarding = null;
        if (hasTable)
        {
            regarding = Reference(regardingTable, regardingId);
            if (regarding == null)
                return (false, null, "RegardingId is not a record id.");
        }

        var ownerProblem = OwnerProblem(owner);
        if (ownerProblem != null)
            return (false, null, ownerProblem);

        if (dueInDays < 0)
            return (false, null, "DueInDays must not be negative.");

        var task = new CrmTask
        {
            Subject = title,
            Description = description,
            RegardingObjectId = regarding,
            ScheduledEnd = dueInDays == null ? null : DateTime.UtcNow.Date.AddDays(dueInDays.Value),
        };
        if (owner != null)
        {
            task.OwnerId = owner;
        }

        var id = dao.Create(task);
        return (true, new EntityReference(CrmTask.EntityLogicalName, id), "Task created.");
    }

    // Makes a user or team the owner of a record. Safe to retry: assigning the same owner twice
    // changes nothing. For the record the rule runs on, a Set field action on ownerid is simpler.
    public (bool Success, bool Value, string Message) AssignTo(string? recordTable, string? recordId, EntityReference? owner)
    {
        var record = Reference(recordTable, recordId);
        if (record == null)
            return (false, false, "RecordTable and RecordId must name a record.");

        if (owner == null)
            return (false, false, "OwnerId is required.");

        var ownerProblem = OwnerProblem(owner);
        if (ownerProblem != null)
            return (false, false, ownerProblem);

        dao.Update(new Entity(record.LogicalName, record.Id) { ["ownerid"] = owner });
        return (true, true, $"Assigned to the {owner.LogicalName} {owner.Name ?? owner.Id.ToString()}.");
    }

    private static EntityReference? Reference(string? table, string? id)
    {
        if (string.IsNullOrWhiteSpace(table) || !Guid.TryParse(id?.Trim(), out var guid) || guid == Guid.Empty)
            return null;

        return new EntityReference(table!.Trim().ToLowerInvariant(), guid);
    }

    private static string? OwnerProblem(EntityReference? owner)
    {
        if (owner == null)
            return null;

        return owner.LogicalName is SystemUser.EntityLogicalName or "team"
            ? null
            : $"The owner must be a user or a team, not a {owner.LogicalName} record.";
    }
}
