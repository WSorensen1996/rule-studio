using Microsoft.Xrm.Sdk;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>One save (or one simulated save) of a record, and the rules of its table/event/stage chain.</summary>
public sealed class EvaluationRequest
{
    public EvaluationRequest(Entity target, string eventName, string stage)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(target, nameof(target));
        Target = target;
        Event = eventName;
        Stage = stage;
    }

    // Current values of the record: the pre-image merged with the columns being saved.
    public Entity Target { get; }

    // create, update or delete (json-formats.md names).
    public string Event { get; }

    // beforeSave or afterSave.
    public string Stage { get; }

    // Values before the save, for the changed operator. Null on create.
    public Entity? PreImage { get; set; }

    // Columns the save changes (Update). Null means every column counts as changed for filtered columns.
    public ISet<string>? ChangedColumns { get; init; }

    // The rules of the chain, in any order; the evaluator orders them by priority, then rule key.
    public IList<CompiledRule> Rules { get; } = new List<CompiledRule>();

    // Rules that must not run, by rule id, with their skip reason (ruleDisabled).
    public IDictionary<Guid, string> Skip { get; } = new Dictionary<Guid, string>();

    // Test run: side-effect functions are mocked, retry delays are skipped, traces carry test details.
    public bool Simulation { get; set; }

    // Test runs only: also mock functions without side effects.
    public bool MockAllFunctions { get; set; }
}
