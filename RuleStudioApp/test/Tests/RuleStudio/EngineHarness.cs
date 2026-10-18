using DataverseLogic.RuleStudio.Engine;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using SharedContext.Dao;
using SharedDomain.RuleStudio.Formats;

namespace Tests.RuleStudio;

/// <summary>
/// In-memory engine runs on one root table ("account") with real column metadata, one column per
/// Dataverse type, so conditions and set-field writes can be tested per type without Dataverse.
/// </summary>
internal static class EngineHarness
{
    /// <summary>Options for <see cref="Run"/>.</summary>
    internal sealed class Setup
    {
        internal IFunctionInvoker Invoker { get; init; } = Substitute.For<IFunctionInvoker>();

        // Records that parent hops read, matched by table and id.
        internal IList<Entity> Related { get; init; } = [];

        internal Func<string, IEnumerable<Entity>> Fetch { get; init; } = _ => [];

        internal bool Simulation { get; init; }

        internal bool MockAll { get; init; }

        internal string Stage { get; init; } = "beforeSave";

        internal string Event { get; init; } = "update";
    }

    internal const string Table = "account";

    // Column → metadata of the in-memory account table.
    private static readonly AttributeMetadata[] Columns =
    [
        new StringAttributeMetadata { LogicalName = "name", MaxLength = 100 },
        new StringAttributeMetadata { LogicalName = "accountnumber", MaxLength = 20 },
        new MemoAttributeMetadata { LogicalName = "description", MaxLength = 2000 },
        new IntegerAttributeMetadata { LogicalName = "numberofemployees", MinValue = 0, MaxValue = 1_000_000 },
        new IntegerAttributeMetadata { LogicalName = "wholenumber", MinValue = int.MinValue, MaxValue = int.MaxValue },
        new BigIntAttributeMetadata { LogicalName = "bignumber" },
        new DecimalAttributeMetadata { LogicalName = "decimalnumber", MinValue = -100, MaxValue = 100 },
        new DoubleAttributeMetadata { LogicalName = "floatnumber", MinValue = -1000, MaxValue = 1000 },
        new MoneyAttributeMetadata { LogicalName = "revenue", MinValue = 0, MaxValue = 1_000_000_000 },
        new BooleanAttributeMetadata { LogicalName = "creditonhold" },
        new PicklistAttributeMetadata { LogicalName = "industrycode" },
        new MultiSelectPicklistAttributeMetadata { LogicalName = "tags" },
        new DateTimeAttributeMetadata { LogicalName = "opendate" },
        new LookupAttributeMetadata { LogicalName = "parentaccountid", Targets = ["account"] },
        new LookupAttributeMetadata { LogicalName = "customerid", Targets = ["account", "contact"] },
        new LookupAttributeMetadata { LogicalName = "ownerid", Targets = ["systemuser", "team"] },
    ];

    internal static Entity Record(params (string Column, object? Value)[] values)
    {
        var record = new Entity(Table, Guid.NewGuid());
        foreach (var (column, value) in values)
        {
            record[column] = value;
        }

        return record;
    }

    internal static CompiledNode Condition(string column, string op, object? value, string aggregate = "none") => new()
    {
        Type = "condition",
        Id = Guid.NewGuid(),
        Path = new FieldPath { Table = Table, Column = column },
        Operator = op,
        Aggregate = aggregate,
        Value = value == null ? null : Fixed(value),
    };

    internal static CompiledNode Group(string op, params CompiledNode[] children)
    {
        var group = new CompiledNode { Type = "group", Id = Guid.NewGuid(), Operator = op };
        foreach (var child in children)
        {
            group.Children.Add(child);
        }

        return group;
    }

    internal static ValueSource Fixed(object? value) => new() { Type = "fixed", Value = value };

    internal static CompiledAction SetField(string column, object? value) =>
        new() { Id = Guid.NewGuid(), Type = "setField", TargetField = column, Value = Fixed(value) };

    internal static CompiledRule Rule(CompiledNode conditions, params CompiledAction[] then) => Rule("TEST.RULE", 100, conditions, then);

    internal static CompiledRule Rule(string key, int priority, CompiledNode conditions, params CompiledAction[] then)
    {
        var rule = new CompiledRule { RuleId = Guid.NewGuid(), RuleKey = key, Conditions = conditions };
        rule.Runtime.Priority = priority;
        rule.Trigger.Table = Table;
        rule.Trigger.Event = "update";
        rule.Trigger.Stage = "beforeSave";
        rule.Runtime.FailurePolicy = "logAndContinue";
        foreach (var action in then)
        {
            rule.Then.Add(action);
        }

        return rule;
    }

    internal static EvaluationResult Evaluate(Entity record, CompiledRule rule, string eventName = "update", Entity? preImage = null, ISet<string>? changedColumns = null) =>
        Evaluate(record, rule, _ => [], eventName, preImage, changedColumns);

    // As above; queries (child paths, aggregates) are answered by rows, which sees the FetchXML.
    internal static EvaluationResult Evaluate(Entity record, CompiledRule rule, Func<string, IEnumerable<Entity>> rows, string eventName = "update", Entity? preImage = null, ISet<string>? changedColumns = null)
    {
        rule.Trigger.Event = eventName;
        var request = new EvaluationRequest(record, eventName, "beforeSave") { PreImage = preImage, ChangedColumns = changedColumns };
        request.Rules.Add(rule);
        return new RuleEvaluator(Dao(rows), Substitute.For<IFunctionInvoker>()).Evaluate(request);
    }

    // A chain of rules on one save at the stage; queries return no rows.
    internal static EvaluationResult EvaluateChain(Entity record, string stage, params CompiledRule[] rules)
    {
        var request = new EvaluationRequest(record, "update", stage);
        foreach (var rule in rules)
        {
            rule.Trigger.Stage = stage;
            request.Rules.Add(rule);
        }

        return new RuleEvaluator(Dao(_ => []), Substitute.For<IFunctionInvoker>()).Evaluate(request);
    }

    // A chain with everything configurable: function invoker, related records read by parent
    // hops, FetchXML rows, test mode, stage and event.
    internal static EvaluationResult Run(Setup setup, Entity record, params CompiledRule[] rules)
    {
        var request = new EvaluationRequest(record, setup.Event, setup.Stage) { Simulation = setup.Simulation, MockAllFunctions = setup.MockAll };
        foreach (var rule in rules)
        {
            rule.Trigger.Stage = setup.Stage;
            rule.Trigger.Event = setup.Event;
            request.Rules.Add(rule);
        }

        return new RuleEvaluator(Dao(setup.Fetch, setup.Related), setup.Invoker).Evaluate(request);
    }

    // A set-field action whose expression always fails to evaluate.
    internal static CompiledAction FailingAction(string column = "description") =>
        new() { Id = Guid.NewGuid(), Type = "setField", TargetField = column, Value = new ValueSource { Type = "expression", Expression = "1 / [missingcolumn] +" } };

    // "matched", "notMatched", or "failed (CODE)" for a rule with this one condition on a record with this value.
    internal static string Outcome(string column, object? actual, string op, object? expected, string aggregate = "none")
    {
        var trace = Evaluate(Record((column, actual)), Rule(Condition(column, op, expected, aggregate))).Traces[0];
        return trace.Error == null ? trace.Outcome! : $"{trace.Outcome} ({trace.Error.Code})";
    }

    // The value a set-field action writes to the column, or "failed (CODE): message".
    internal static object? Written(string column, object? value)
    {
        var result = Evaluate(Record(), Rule(Group("and"), SetField(column, value)), _ => []);
        var trace = result.Traces[0];
        if (trace.Error != null)
            return $"failed ({trace.Error.Code}): {trace.Error.Message}";

        return result.Changes.TryGetValue(column, out var written) ? written : "not written";
    }

    // A data access object that answers metadata requests for the in-memory account table, and
    // FetchXML queries with the given rows.
    private static IDataverseAccessObject Dao(Func<string, IEnumerable<Entity>> rows, IList<Entity>? related = null)
    {
        var dao = Substitute.For<IDataverseAccessObject>();
        var table = new EntityMetadata { LogicalName = Table };
        typeof(EntityMetadata).GetProperty(nameof(EntityMetadata.Attributes))!.SetValue(table, Columns);
        var metadata = new RetrieveMetadataChangesResponse();
        metadata.Results["EntityMetadata"] = new EntityMetadataCollection { table };
        dao.Execute(Arg.Any<OrganizationRequest>(), Arg.Any<string>()).Returns(call =>
        {
            var response = new RetrieveMultipleResponse();
            switch (call.Arg<OrganizationRequest>())
            {
                case RetrieveMultipleRequest { Query: FetchExpression fetch }:
                    response.Results["EntityCollection"] = new EntityCollection(rows(fetch.Query).ToList());
                    return response;
                case RetrieveMultipleRequest { Query: QueryExpression query }:
                    var id = query.Criteria.Conditions.Select(c => c.Values.FirstOrDefault()).OfType<Guid>().FirstOrDefault();
                    var found = (related ?? []).Where(e => e.LogicalName == query.EntityName && e.Id == id).ToList();
                    response.Results["EntityCollection"] = new EntityCollection(found);
                    return response;
                default:
                    return metadata;
            }
        });
        return dao;
    }
}
