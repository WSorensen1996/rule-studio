using DataverseLogic;
using Microsoft.Xrm.Sdk;
using SharedContext.Dao;

namespace RuleStudio.Engine;

/// <summary>Applies a chain's field changes to the triggering record.</summary>
public static class ChangeApplier
{
    // Before save: the changes go into the Target being saved, whatever the identity of the rules.
    public static void ApplyToTarget(Entity target, ChainOutcome outcome)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(target, nameof(target));
        ArgumentNullExceptionHelper.ThrowIfNull(outcome, nameof(outcome));
        foreach (var change in outcome.Changes)
        {
            target[change.Key] = change.Value.Value;
        }
    }

    // After save: one Update per identity, so system rules can set columns the user may not write.
    // Returns the error message when an update failed; the record is already saved, so nothing is blocked.
    public static string? UpdateRecord(string table, Guid id, ChainOutcome outcome, Func<bool, IDataverseAccessObject> daoFor)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(outcome, nameof(outcome));
        ArgumentNullExceptionHelper.ThrowIfNull(daoFor, nameof(daoFor));
        foreach (var asSystem in new[] { true, false })
        {
            var update = new Entity(table, id);
            foreach (var change in outcome.Changes.Where(c => c.Value.AsSystem == asSystem))
            {
                update[change.Key] = change.Value.Value;
            }

            if (update.Attributes.Count == 0)
                continue;

            try
            {
                daoFor(asSystem).Update(update);
            }
#pragma warning disable CA1031 // After save the failure is logged and flagged for an admin.
            catch (Exception ex)
#pragma warning restore CA1031
            {
                return ex.Message;
            }
        }

        return null;
    }
}
