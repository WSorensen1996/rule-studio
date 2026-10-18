using Microsoft.Xrm.Sdk.Metadata;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Release;

/// <summary>Checks a value source: field paths, expressions, lookups and functions with their input mappings.</summary>
internal static class ValueChecks
{
    // Checks the source. For a field path returns its column, for other sources null.
    internal static AttributeMetadata? Check(CheckContext context, ValueSource? source, Guid ownerId, Location at)
    {
        switch (source?.Type)
        {
            case null:
                return null;
            case "fieldPath":
                var column = PathChecks.Check(context, source.Path, at with { Label = $"{at.Label} value" }, out var child);
                if (child)
                {
                    context.Add(CheckContext.Error, ValidationCodes.ChildPathWithoutAggregate, $"{at.Label} takes its value from child records, which needs an aggregate.", at);
                }

                return column;
            case "expression":
                PathChecks.CheckExpression(context, source.Expression, at);
                return null;
            case "lookup":
                CheckLookup(context, source.LookupDefinitionId, at);
                return null;
            case "function":
                CheckFunction(context, source.CustomFunctionId, ownerId, at);
                return null;
            default:
                return null;
        }
    }

    private static void CheckLookup(CheckContext context, Guid? id, Location at)
    {
        var fail = (string why) => context.Add(CheckContext.Error, ValidationCodes.LookupInvalid, $"{at.Label}: {why}", at);
        if (id == null || !context.Input.Lookups.TryGetValue(id.Value, out var lookup))
        {
            fail("the lookup definition is missing or was deleted.");
            return;
        }

        var name = lookup.rs_Name ?? "the lookup";
        var metadata = context.Metadata;
        if (metadata.Table(lookup.rs_LookupTable) == null)
        {
            fail($"{name} reads table {lookup.rs_LookupTable}, which does not exist.");
            return;
        }

        foreach (var column in new[] { lookup.rs_ReturnColumn, lookup.rs_SortColumn }.Where(c => !string.IsNullOrWhiteSpace(c)))
        {
            if (metadata.Column(lookup.rs_LookupTable, column) == null)
            {
                fail($"{name} uses column {lookup.rs_LookupTable}.{column}, which does not exist.");
            }
        }

        var criteria = Json.Read<MatchCriteria>(lookup.rs_MatchCriteria);
        if (criteria == null || criteria.Version != FormatVersion.Current)
        {
            fail($"{name} has match criteria that cannot be read.");
        }
        else if (!string.Equals(criteria.RecordTable, context.Table, StringComparison.OrdinalIgnoreCase))
        {
            fail($"{name} is defined for {criteria.RecordTable} records, but the rule targets {context.Table}.");
        }
        else
        {
            CheckCriteria(context, lookup.rs_LookupTable!, criteria, at with { Label = $"{at.Label}: {name}" });
        }
    }

    private static void CheckCriteria(CheckContext context, string lookupTable, MatchCriteria criteria, Location at)
    {
        foreach (var criterion in criteria.Criteria)
        {
            foreach (var column in new[] { criterion.Column, criterion.MinColumn, criterion.MaxColumn }.Where(c => !string.IsNullOrWhiteSpace(c)))
            {
                if (context.Metadata.Column(lookupTable, column) == null)
                {
                    context.Add(CheckContext.Error, ValidationCodes.LookupInvalid, $"{at.Label} matches on {lookupTable}.{column}, which does not exist.", at);
                }
            }

            foreach (var value in new[] { criterion.Value }.Concat(criterion.Values).OfType<ValueSource>())
            {
                if (value.Type is "lookup" or "function")
                {
                    context.Add(CheckContext.Error, ValidationCodes.LookupInvalid, $"{at.Label} uses a {value.Type} inside its match criteria, which is not allowed.", at);
                }
                else
                {
                    Check(context, value, Guid.Empty, at);
                }
            }
        }
    }

    private static void CheckFunction(CheckContext context, Guid? id, Guid ownerId, Location at)
    {
        context.FunctionCalls++;
        if (id == null || !context.Input.Functions.TryGetValue(id.Value, out var function) || function.rs_Enabled == false)
        {
            context.Add(CheckContext.Error, ValidationCodes.FunctionNotRegistered, $"{at.Label} calls a custom function that is missing or disabled.", at);
            return;
        }

        var name = function.rs_Name ?? function.rs_CustomApiName;
        if (function.rs_HasSideEffects == true)
        {
            context.Add(CheckContext.Warning, ValidationCodes.SideEffectFunction, $"{at.Label} calls {name}, which has side effects; it is mocked when testing.", at);
        }

        var schema = Json.Read<InputSchema>(function.rs_InputSchema);
        if (schema == null)
        {
            context.Add(CheckContext.Error, ValidationCodes.InputMappingInvalid, $"{at.Label}: the input schema of {name} cannot be read. Fix it on the Custom functions page.", at);
            return;
        }

        CheckInputs(context, schema, ownerId, at with { Label = $"{at.Label} ({name})" });
    }

    private static void CheckInputs(CheckContext context, InputSchema schema, Guid ownerId, Location at)
    {
        var mappings = context.Draft.MappingsOf(ownerId).ToList();
        foreach (var parameter in schema.Parameters.Where(p => p.Required))
        {
            if (!mappings.Exists(m => m.rs_ParameterName == parameter.Name))
            {
                context.Add(CheckContext.Error, ValidationCodes.RequiredInputMissing, $"{at.Label}: required input {parameter.DisplayName ?? parameter.Name} has no value.", at);
            }
        }

        foreach (var mapping in mappings)
        {
            var location = new Location("inputMapping", mapping.Id, $"{at.Label} input {mapping.rs_ParameterName}");
            if (!schema.Parameters.Any(p => p.Name == mapping.rs_ParameterName))
            {
                context.Add(CheckContext.Error, ValidationCodes.InputMappingInvalid, $"{location.Label} is not an input of the function.", location);
                continue;
            }

            var source = Json.Source(mapping.rs_ValueSourceType, mapping.rs_Value, mapping.rs_ValuePath, (null, null));
            if (source?.Type is "lookup" or "function")
            {
                context.Add(CheckContext.Error, ValidationCodes.InputMappingInvalid, $"{location.Label} uses a {source.Type}, which inputs do not support.", location);
            }
            else
            {
                Check(context, source, mapping.Id, location);
            }
        }
    }
}
