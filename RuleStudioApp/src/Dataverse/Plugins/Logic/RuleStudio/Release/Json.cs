using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuleStudio.SharedContext;
using SharedDomain.RuleStudio.Formats;

namespace DataverseLogic.RuleStudio.Release;

/// <summary>Reading and writing the JSON columns (docs/json-formats.md).</summary>
internal static class Json
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        DateTimeZoneHandling = DateTimeZoneHandling.Utc,
        DateFormatString = "yyyy-MM-dd'T'HH:mm:ss'Z'",
        Formatting = Formatting.None,
    };

    internal static string Write(object document) => JsonConvert.SerializeObject(document, Settings);

    // Lowercase hex SHA-256 of the UTF-8 text.
    internal static string Hash(string text)
    {
#pragma warning disable CA1850 // SHA256.HashData does not exist on net462.
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
#pragma warning restore CA1850
        return string.Concat(bytes.Select(b => b.ToString("x2", System.Globalization.CultureInfo.InvariantCulture)));
    }

    // Parses a JSON document; null when the text is empty or not valid.
    internal static T? Read<T>(string? text)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        try
        {
            return JsonConvert.DeserializeObject<T>(text!, Settings);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    // rs_value for Fixed: JSON, or a single string when it is not valid JSON (json-formats.md, Decisions 6).
    internal static JToken FixedValue(string? text)
    {
        if (text == null)
            return JValue.CreateNull();

        try
        {
            return JToken.Parse(text);
        }
        catch (JsonReaderException)
        {
            return new JValue(text);
        }
    }

    // The value source stored in a row's rs_valuesourcetype, rs_value, rs_valuepath and lookups.
    internal static ValueSource? Source(
        rs_valuesourcetype? type,
        string? value,
        string? valuePath,
        (Guid? Lookup, Guid? Function) references)
    {
        return type switch
        {
            null => null,
            rs_valuesourcetype.Fieldpath => new ValueSource { Type = "fieldPath", Path = Read<FieldPath>(valuePath) },
            rs_valuesourcetype.Expression => new ValueSource { Type = "expression", Expression = value ?? string.Empty },
            rs_valuesourcetype.Lookup => new ValueSource { Type = "lookup", LookupDefinitionId = references.Lookup },
            rs_valuesourcetype.Function => new ValueSource { Type = "function", CustomFunctionId = references.Function },
            _ => new ValueSource { Type = "fixed", Value = FixedValue(value) },
        };
    }
}
