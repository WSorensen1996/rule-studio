using System.Text.RegularExpressions;
using DataverseLogic.RuleStudio.Engine;
using Microsoft.Xrm.Sdk.Metadata;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Release;

/// <summary>Checks field paths (json-formats.md "Field path") and expression field references (json-formats.md, Decisions 1).</summary>
internal static class PathChecks
{
    // Maximum hops in one path; the format has no limit, publishing does.
    internal const int MaxHops = EngineLimits.MaxHops;

    // name( outside strings and field references: a function call.
    private static readonly Regex FunctionCall = new(@"(?<name>[A-Za-z_]\w*)\s*\(", RegexOptions.Compiled | RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(1));
    private static readonly Regex Literal = new(@"'(?:[^'\\]|\\.)*'|\[[^\[\]]*\]", RegexOptions.Compiled | RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(1));

    private static readonly Regex FieldReference = new(@"\[(?<ref>[^\[\]]*)\]", RegexOptions.Compiled | RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(1));

    // Checks a path from the rule's table. Returns the final column, or null when the path is broken.
    internal static AttributeMetadata? Check(CheckContext context, FieldPath? path, Location at, out bool hasChildHop)
    {
        hasChildHop = false;
        if (path == null || string.IsNullOrWhiteSpace(path.Column))
        {
            context.Add(CheckContext.Error, ValidationCodes.FieldNotFound, $"{at.Label} has no field selected.", at);
            return null;
        }

        if (!string.Equals(path.Table, context.Table, StringComparison.OrdinalIgnoreCase))
        {
            context.Add(CheckContext.Error, ValidationCodes.FieldNotFound, $"{at.Label} starts at table {path.Table}, but the rule targets {context.Table}.", at);
            return null;
        }

        if (path.Hops.Count > MaxHops)
        {
            context.Add(CheckContext.Error, ValidationCodes.HopLimitExceeded, $"{at.Label} follows {path.Hops.Count} related tables; at most {MaxHops} are allowed.", at);
        }

        var table = path.Table;
        foreach (var hop in path.Hops)
        {
            if (!HopExists(context.Metadata, table, hop))
            {
                context.Add(CheckContext.Error, ValidationCodes.FieldNotFound, $"{at.Label} follows {hop.Attribute} to {hop.Table}, which is not a lookup relationship Rule Studio can follow. Pick the related table through its own lookup instead.", at);
                return null;
            }

            hasChildHop |= hop.Kind == "child";
            table = hop.Table;
        }

        context.Hops += path.Hops.Count;
        var column = context.Metadata.Column(table, path.Column);
        if (column == null)
        {
            context.Add(CheckContext.Error, ValidationCodes.FieldNotFound, $"{at.Label} reads {table}.{path.Column}, which no longer exists.", at);
        }

        return column;
    }

    // Checks the bracketed field references and the bracket balance of an NCalc expression.
    internal static void CheckExpression(CheckContext context, string? expression, Location at)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            context.Add(CheckContext.Error, ValidationCodes.ExpressionInvalid, $"{at.Label} has an empty expression.", at);
            return;
        }

        if (!Balanced(expression!))
        {
            context.Add(CheckContext.Error, ValidationCodes.ExpressionInvalid, $"{at.Label} has unbalanced brackets or parentheses.", at);
            return;
        }

        var parsed = new NCalc.Expression(expression, ExpressionEvaluator.Options);
        if (parsed.HasErrors())
        {
            context.Add(CheckContext.Error, ValidationCodes.ExpressionInvalid, $"{at.Label} is not a valid formula: {parsed.Error}", at);
            return;
        }

        foreach (var reference in FieldReference.Matches(expression!).Cast<Match>().Select(m => m.Groups["ref"].Value))
        {
            var problem = ResolveReference(context, reference);
            if (problem != null)
            {
                context.Add(CheckContext.Error, ValidationCodes.ExpressionInvalid, $"{at.Label}: [{reference}] {problem}.", at);
            }
        }

        var code = Literal.Replace(expression!, " ");
        foreach (var name in FunctionCall.Matches(code).Cast<Match>().Select(m => m.Groups["name"].Value).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!ExpressionEvaluator.BuiltIns.Contains(name) && !context.Input.FunctionNames.Contains(name))
            {
                context.Add(CheckContext.Error, ValidationCodes.FunctionNotRegistered, $"{at.Label} calls {name}(), which is neither an NCalc function nor an enabled custom function.", at);
            }
        }
    }

    private static bool HopExists(TableMetadata metadata, string fromTable, FieldPathHop hop)
    {
        // Parent: the lookup is on the current table. Child: the lookup is on the child table and points back.
        var lookupTable = hop.Kind == "child" ? hop.Table : fromTable;
        var pointsTo = hop.Kind == "child" ? fromTable : hop.Table;
        return metadata.Table(hop.Table) != null
            && metadata.Column(lookupTable, hop.Attribute) is LookupAttributeMetadata lookup
            && (lookup.Targets == null || lookup.Targets.Contains(pointsTo, StringComparer.OrdinalIgnoreCase));
    }

    // Null when the reference resolves, otherwise why it does not.
    private static string? ResolveReference(CheckContext context, string reference)
    {
        var segments = reference.Split('.');
        var table = context.Table;
        for (var i = 0; i < segments.Length - 1; i++)
        {
            var parts = segments[i].Split(':');
            if (context.Metadata.Column(table, parts[0]) is not LookupAttributeMetadata lookup)
                return $"refers to {table}.{parts[0]}, which is not a lookup column";

            var targets = lookup.Targets ?? [];
            if (parts.Length == 1 && targets.Length != 1)
                return $"must name the table of polymorphic lookup {parts[0]}, e.g. {parts[0]}:{targets.FirstOrDefault() ?? "account"}";
            table = parts.Length > 1 ? parts[1] : targets[0];
        }

        context.Hops += segments.Length - 1;
        return context.Metadata.Column(table, segments[segments.Length - 1]) == null ? $"refers to {table}.{segments[segments.Length - 1]}, which does not exist" : null;
    }

    private static bool Balanced(string expression)
    {
        int brackets = 0, parentheses = 0;
        var quoted = false;
        foreach (var c in expression)
        {
            if (c == '\'')
            {
                quoted = !quoted;
            }

            if (quoted)
                continue;

            brackets += Step(c, '[', ']');
            parentheses += Step(c, '(', ')');
            if (brackets is < 0 or > 1 || parentheses < 0)
                return false;
        }

        return !quoted && brackets == 0 && parentheses == 0;
    }

    private static int Step(char c, char open, char close)
    {
        if (c == open)
            return 1;
        return c == close ? -1 : 0;
    }
}
