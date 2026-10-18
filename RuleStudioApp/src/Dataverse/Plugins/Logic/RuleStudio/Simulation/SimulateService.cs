using System.Globalization;
using DataverseLogic.RuleStudio.Engine;
using DataverseLogic.RuleStudio.Release;
using Microsoft.Xrm.Sdk;
using RuleStudio.SharedContext;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Simulation;

/// <summary>
/// rs_SimulateRule (docs/custom-apis.md). Runs drafts or published versions against a
/// real or sample record with the rule evaluator and returns the traces. Nothing is saved: the
/// evaluator never writes, side-effect functions are mocked, and no execution log is written.
/// Runs as the calling user, so it reads only what the caller may read.
/// </summary>
public class SimulateService(IUserDataverseAccessObjectService dao)
{
    private readonly IUserDataverseAccessObjectService dao = dao;

    // Returns a JSON array of rs_trace documents (json-formats.md §4), one per rule in chain order.
    public string Simulate(SimulationRequest request)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(request, nameof(request));
        if (request.LookupDefinitionId is { } lookupId && lookupId != Guid.Empty)
            return Json.Write(TestLookup(request, lookupId));

        var selector = new RuleSelector(dao);
        var chain = string.IsNullOrWhiteSpace(request.RuleIds)
            ? selector.ByTrigger(Required(request.Table, "Table"), EventName(request.Event), StageName(request.Stage), request.UseDrafts)
            : selector.ByIds(ReleaseService.ParseIds(request.RuleIds!), request.UseDrafts);
        var record = SampleRecord.Build(dao, chain, request.RecordId, request.SampleValues);

        var evaluation = new EvaluationRequest(record.Target, chain.Event, chain.Stage)
        {
            PreImage = record.PreImage,
            ChangedColumns = record.Changed,
            Simulation = true,
            MockAllFunctions = request.MockFunctions,
        };
        foreach (var rule in chain.Rules)
        {
            evaluation.Rules.Add(rule);
        }

        foreach (var skip in chain.Skip)
        {
            evaluation.Skip[skip.Key] = skip.Value;
        }

        var result = new RuleEvaluator(dao, new CustomApiInvoker(dao)).Evaluate(evaluation);
        foreach (var rule in result.Traces.Select(t => t.Rule).OfType<TraceRule>())
        {
            rule.Name = chain.Names.TryGetValue(rule.RuleId, out var name) ? name : null;
            rule.Draft = chain.Drafts.Contains(rule.RuleId);
        }

        return Json.Write(result.Traces);
    }

    // One lookup definition against a real or sample record of Table.
    private LookupTestResult TestLookup(SimulationRequest request, Guid lookupId)
    {
        var definition = dao.RetrieveMultipleByIds<rs_LookupDefinition>([lookupId]).FirstOrDefault()
            ?? throw new InvalidPluginExecutionException($"Lookup definition {lookupId} does not exist.");
        var chain = new SimulatedChain { Table = Required(request.Table, "Table"), Event = "create", Stage = "beforeSave" };
        var record = SampleRecord.Build(dao, chain, request.RecordId, request.SampleValues);
        var evaluation = new EvaluationRequest(record.Target, chain.Event, chain.Stage) { Simulation = true };
        return new RuleEvaluator(dao, new CustomApiInvoker(dao)).TestLookup(evaluation, SnapshotCompiler.Lookup(definition));
    }

    private static string Required(string? value, string name) =>
        string.IsNullOrWhiteSpace(value) ? throw new InvalidPluginExecutionException($"Give either RuleIds or {name}, Event and Stage.") : value!.Trim().ToLowerInvariant();

    // create/update/delete, or the rs_event choice value.
    private static string EventName(string? value)
    {
        var text = Required(value, "Event");
        var name = int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var choice) ? ChoiceNames.Event((rs_event)choice) : text;
        return name is "create" or "update" or "delete" ? name : throw new InvalidPluginExecutionException($"Event {value} is not create, update or delete.");
    }

    // beforeSave/afterSave, or the rs_stage choice value.
    private static string StageName(string? value)
    {
        var text = Required(value, "Stage");
        if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var choice))
            return ChoiceNames.Stage((rs_stage)choice) is { Length: > 0 } name ? name : throw new InvalidPluginExecutionException($"Stage {value} is not a stage.");

        return text switch
        {
            "beforesave" => "beforeSave",
            "aftersave" => "afterSave",
            _ => throw new InvalidPluginExecutionException($"Stage {value} is not beforeSave or afterSave."),
        };
    }
}
