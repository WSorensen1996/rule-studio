using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Engine;

/// <summary>Converts normalized values back to the SDK types Dataverse expects for a column or Custom API parameter.</summary>
internal static class Coerce
{
    // The value as the column stores it; unchanged when the column is unknown. A value the column
    // cannot hold (too long, out of range, wrong lookup table) fails with a readable message.
    internal static object? ToColumn(object? value, AttributeMetadata? column)
    {
        if (Values.IsEmpty(value))
            return null;

        if (column is MultiSelectPicklistAttributeMetadata)
            return new OptionSetValueCollection(Values.AsList(value).Select(v => new OptionSetValue((int)WholeNumber(v, column, int.MinValue, int.MaxValue))).ToList());

        return column?.AttributeType switch
        {
            null => value,
            AttributeTypeCode.Money => new Money(InRange(Number(value, column), column)),
            AttributeTypeCode.Decimal => InRange(Number(value, column), column),
            AttributeTypeCode.Double => (double)InRange(Number(value, column), column),
            AttributeTypeCode.Integer => (int)WholeNumber(value, column, int.MinValue, int.MaxValue),
            AttributeTypeCode.BigInt => WholeNumber(value, column, long.MinValue, long.MaxValue),
            AttributeTypeCode.Boolean => Flag(value),
            AttributeTypeCode.Picklist or AttributeTypeCode.State or AttributeTypeCode.Status => new OptionSetValue((int)WholeNumber(value, column, int.MinValue, int.MaxValue)),
            AttributeTypeCode.Lookup or AttributeTypeCode.Customer or AttributeTypeCode.Owner => LookupReference(value, (LookupAttributeMetadata)column),
            AttributeTypeCode.DateTime => Date(value),
            AttributeTypeCode.String or AttributeTypeCode.Memo => Text(value, column),
            AttributeTypeCode.Uniqueidentifier => value is EntityReference r ? r.Id : Guid.Parse(Values.Text(value)),
            _ => value,
        };
    }

    // Half away from zero, as Excel rounds: 2.5 → 3, −2.5 → −3.
    internal static decimal Round(decimal value) => decimal.Round(value, MidpointRounding.AwayFromZero);

    // The value as a query condition expects it for the column: plain numbers, ids and texts.
    internal static object? ToQuery(object? value, AttributeMetadata? column) => ToColumn(value, column) switch
    {
        Money money => money.Value,
        OptionSetValue option => option.Value,
        EntityReference reference => reference.Id,
        var plain => plain,
    };

    // The value as a Custom API request parameter of the schema type (json-formats.md §2).
    internal static object? ToParameter(object? value, FunctionParameter? parameter)
    {
        if (Values.IsEmpty(value))
            return null;

        return parameter?.Type switch
        {
            "text" => Values.Text(value),
            "number" => (int)Round(Values.Number(value) ?? 0),
            "decimal" => Values.Number(value) ?? 0,
            "money" => new Money(Values.Number(value) ?? 0),
            "boolean" => Flag(value),
            "date" => Date(value),
            "lookup" => Reference(value, parameter.Table),
            _ => value is IList<object?> list ? Values.Text(list) : value,
        };
    }

    private static decimal Number(object? value, AttributeMetadata column) =>
        Values.Number(value) ?? throw new EngineException(EngineCodes.RuleFailed, $"{Values.Text(value)} is not a number, so it cannot be written to {column.LogicalName}.");

    // A rounded whole number within the column's range and the SDK type's range.
    private static long WholeNumber(object? value, AttributeMetadata column, long typeMin, long typeMax)
    {
        var number = InRange(Round(Number(value, column)), column);
        if (number < typeMin || number > typeMax)
            throw OutOfRange(number, column, typeMin, typeMax);

        return (long)number;
    }

    // The number, or a readable error when it is outside the column's minimum and maximum.
    private static decimal InRange(decimal number, AttributeMetadata column)
    {
        var (min, max) = column switch
        {
            IntegerAttributeMetadata x => ((decimal?)x.MinValue, (decimal?)x.MaxValue),
            BigIntAttributeMetadata x => (x.MinValue, x.MaxValue),
            DecimalAttributeMetadata x => (x.MinValue, x.MaxValue),
            DoubleAttributeMetadata x => ((decimal?)x.MinValue, (decimal?)x.MaxValue),
            MoneyAttributeMetadata x => ((decimal?)x.MinValue, (decimal?)x.MaxValue),
            _ => (null, null),
        };
        if ((min.HasValue && number < min.Value) || (max.HasValue && number > max.Value))
            throw OutOfRange(number, column, min, max);

        return number;
    }

    private static EngineException OutOfRange(decimal number, AttributeMetadata column, decimal? min, decimal? max) =>
        new(EngineCodes.RuleFailed, $"{number.ToString(CultureInfo.InvariantCulture)} is outside the range of {column.LogicalName} ({Bound(min)} to {Bound(max)}).");

    private static string Bound(decimal? value) => value?.ToString("0.##########", CultureInfo.InvariantCulture) ?? "no limit";

    private static string Text(object? value, AttributeMetadata column)
    {
        var text = Values.Text(value);
        var max = (column as StringAttributeMetadata)?.MaxLength ?? (column as MemoAttributeMetadata)?.MaxLength;
        if (max is > 0 && text.Length > max.Value)
            throw new EngineException(EngineCodes.RuleFailed, $"The text is {text.Length} characters; {column.LogicalName} allows {max.Value}.");

        return text;
    }

    // A lookup value keeps its own table. A bare id needs the column to point to exactly one table.
    private static EntityReference LookupReference(object? value, LookupAttributeMetadata column)
    {
        var targets = column.Targets ?? [];
        if (value is EntityReference reference)
        {
            if (targets.Length > 0 && !targets.Contains(reference.LogicalName, StringComparer.OrdinalIgnoreCase))
                throw new EngineException(EngineCodes.RuleFailed, $"{column.LogicalName} cannot point to {reference.LogicalName} records; it allows {string.Join(" or ", targets)}.");

            return reference;
        }

        if (targets.Length > 1 && Guid.TryParse(Values.Text(value), out _))
            throw new EngineException(EngineCodes.RuleFailed, $"{column.LogicalName} can point to {string.Join(" or ", targets)}; give the table, not only the id.");

        return Reference(value, targets.FirstOrDefault());
    }

    private static bool Flag(object? value) => value switch
    {
        bool flag => flag,
        decimal number => number != 0,
        _ => bool.TryParse(Values.Text(value), out var parsed) && parsed,
    };

    private static DateTime Date(object? value)
    {
        if (value is DateTime date)
            return date;

        return DateTime.Parse(Values.Text(value), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
    }

    private static EntityReference Reference(object? value, string? table)
    {
        if (value is EntityReference reference)
            return reference;

        if (Guid.TryParse(Values.Text(value), out var id) && !string.IsNullOrEmpty(table))
            return new EntityReference(table, id);

        throw new EngineException(EngineCodes.RuleFailed, $"{Values.Text(value)} is not a record reference.");
    }
}
