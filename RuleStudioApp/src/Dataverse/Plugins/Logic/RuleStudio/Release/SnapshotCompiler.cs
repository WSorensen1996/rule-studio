using RuleStudio.SharedContext;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Release;

/// <summary>Compiles a version's authoring rows into rs_compiledsnapshot (json-formats.md §5).</summary>
internal static class SnapshotCompiler
{
    internal static CompiledRule Compile(RuleDraft draft, ReleaseInput input)
    {
        var version = draft.Version;
        var compiled = new CompiledRule
        {
            RuleId = draft.Rule.Id,
            RuleKey = draft.RuleKey,
            RuleVersionId = version.Id,
            VersionNumber = version.rs_VersionNumber ?? 0,
            Trigger = Trigger(version),
            Runtime = new RuleRuntime
            {
                Priority = version.rs_Priority ?? 0,
                StopProcessing = version.rs_StopProcessing ?? false,
                FailurePolicy = ChoiceNames.FailurePolicy(version.rs_FailurePolicy),
                RunAs = ChoiceNames.RunAs(version.rs_RunAs),
                RetryCount = version.rs_RetryCount ?? 0,
                RetryDelayMs = version.rs_RetryDelayMs ?? 0,
                LogLevel = ChoiceNames.LogLevel(version.rs_LogLevel),
            },
            Conditions = RootGroup(draft),
        };
        foreach (var action in draft.Actions.OrderBy(a => a.rs_Order ?? 0))
        {
            var target = action.rs_Branch == rs_action_rs_branch.Else ? compiled.Else : compiled.Then;
            target.Add(Action(draft, action));
        }

        AddReferences(compiled, draft, input);
        return compiled;
    }

    internal static IEnumerable<string> FilteredColumns(rs_RuleVersion version) =>
        (version.rs_FilteredColumns ?? string.Empty)
            .Split([',', ';', ' '], StringSplitOptions.RemoveEmptyEntries)
            .Select(c => c.Trim().ToLowerInvariant());

    private static RuleTrigger Trigger(rs_RuleVersion version)
    {
        var trigger = new RuleTrigger
        {
            Table = version.rs_TargetTable ?? string.Empty,
            Event = ChoiceNames.Event(version.rs_Event),
            Stage = ChoiceNames.Stage(version.rs_Stage),
        };
        if (version.rs_Event == rs_event.Update)
        {
            foreach (var column in FilteredColumns(version))
            {
                trigger.FilteredColumns.Add(column);
            }
        }

        return trigger;
    }

    private static CompiledNode RootGroup(RuleDraft draft)
    {
        var root = draft.Groups.Find(g => g.rs_ParentGroupId == null);
        return root == null ? new CompiledNode { Type = "group", Operator = "and" } : Group(draft, root);
    }

    private static CompiledNode Group(RuleDraft draft, rs_ConditionGroup group)
    {
        var node = new CompiledNode { Type = "group", Id = group.Id, Operator = ChoiceNames.Logic(group.rs_LogicalOperator) };
        var children = draft.Groups.Where(g => g.rs_ParentGroupId?.Id == group.Id)
            .Select(g => (Order: g.rs_Order ?? 0, Node: Group(draft, g)))
            .Concat(draft.Conditions.Where(c => c.rs_ConditionGroupId?.Id == group.Id)
                .Select(c => (Order: c.rs_Order ?? 0, Node: Condition(draft, c))))
            .OrderBy(c => c.Order);
        foreach (var child in children)
        {
            node.Children.Add(child.Node);
        }

        return node;
    }

    private static CompiledNode Condition(RuleDraft draft, rs_Condition condition)
    {
        var node = new CompiledNode
        {
            Type = "condition",
            Id = condition.Id,
            Path = Json.Read<FieldPath>(condition.rs_FieldPath),
            Aggregate = ChoiceNames.Aggregate(condition.rs_Aggregate),
            Operator = ChoiceNames.Operator(condition.rs_Operator),
            Value = Json.Source(condition.rs_ValueSourceType, condition.rs_Value, condition.rs_ValuePath, (condition.rs_LookupDefinitionId?.Id, condition.rs_CustomFunctionId?.Id)),
        };
        if (condition.rs_ValueSourceType == rs_valuesourcetype.Function)
        {
            AddInputs(node.Inputs, draft, condition.Id);
        }

        return node;
    }

    private static CompiledAction Action(RuleDraft draft, rs_Action action)
    {
        var compiled = new CompiledAction
        {
            Id = action.Id,
            Type = ChoiceNames.ActionType(action.rs_ActionType),
            TargetField = action.rs_ActionType == rs_action_rs_actiontype.Setfield ? action.rs_TargetField : null,
            Message = action.rs_Message,
            MessageLevel = action.rs_ActionType == rs_action_rs_actiontype.Logmessage ? ChoiceNames.MessageLevel(action.rs_MessageLevel) : null,
        };
        if (action.rs_ActionType is rs_action_rs_actiontype.Setfield or rs_action_rs_actiontype.Callfunction)
        {
            compiled.Value = Json.Source(action.rs_ValueSourceType, action.rs_Value, action.rs_ValuePath, (action.rs_LookupDefinitionId?.Id, action.rs_CustomFunctionId?.Id));
        }

        if (action.rs_ValueSourceType == rs_valuesourcetype.Function)
        {
            AddInputs(compiled.Inputs, draft, action.Id);
        }

        return compiled;
    }

    private static void AddInputs(IList<CompiledInput> inputs, RuleDraft draft, Guid ownerId)
    {
        foreach (var mapping in draft.MappingsOf(ownerId).OrderBy(m => m.rs_ParameterName, StringComparer.Ordinal))
        {
            inputs.Add(new CompiledInput
            {
                Name = mapping.rs_ParameterName ?? string.Empty,
                Value = Json.Source(mapping.rs_ValueSourceType, mapping.rs_Value, mapping.rs_ValuePath, (null, null)),
            });
        }
    }

    private static void AddReferences(CompiledRule compiled, RuleDraft draft, ReleaseInput input)
    {
        var lookupIds = draft.Conditions.Select(c => c.rs_LookupDefinitionId?.Id)
            .Concat(draft.Actions.Select(a => a.rs_LookupDefinitionId?.Id)).OfType<Guid>().Distinct().OrderBy(id => id);
        foreach (var id in lookupIds)
        {
            if (input.Lookups.TryGetValue(id, out var lookup))
            {
                compiled.Lookups.Add(Lookup(lookup));
            }
        }

        var functionIds = draft.Conditions.Select(c => c.rs_CustomFunctionId?.Id)
            .Concat(draft.Actions.Select(a => a.rs_CustomFunctionId?.Id)).OfType<Guid>().Distinct().OrderBy(id => id);
        foreach (var id in functionIds)
        {
            if (input.Functions.TryGetValue(id, out var function))
            {
                compiled.Functions.Add(Function(function));
            }
        }
    }

    internal static CompiledLookup Lookup(rs_LookupDefinition lookup) => new()
    {
        Id = lookup.Id,
        Name = lookup.rs_Name ?? string.Empty,
        Table = lookup.rs_LookupTable ?? string.Empty,
        ReturnColumn = lookup.rs_ReturnColumn ?? string.Empty,
        SortColumn = lookup.rs_SortColumn,
        NoMatch = ChoiceNames.NoMatch(lookup.rs_NoMatchPolicy),
        DefaultValue = lookup.rs_DefaultValue == null ? null : Json.FixedValue(lookup.rs_DefaultValue),
        MultiMatch = ChoiceNames.MultiMatch(lookup.rs_MultiMatchPolicy),
        MatchCriteria = Json.Read<MatchCriteria>(lookup.rs_MatchCriteria),
    };

    private static CompiledFunction Function(rs_CustomFunction function) => new()
    {
        Id = function.Id,
        Name = function.rs_Name ?? string.Empty,
        ApiName = function.rs_CustomApiName ?? string.Empty,
        OutputType = ChoiceNames.OutputType(function.rs_OutputType),
        HasSideEffects = function.rs_HasSideEffects ?? false,
        SafeToRetry = function.rs_SafeToRetry ?? false,
        TimeoutMs = function.rs_TimeoutMs,
        InputSchema = Json.Read<InputSchema>(function.rs_InputSchema),
    };
}
