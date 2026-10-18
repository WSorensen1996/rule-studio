using DataverseLogic;
using Newtonsoft.Json;
using SharedDomain.RuleStudio.Formats;

namespace RuleStudio.Engine;

/// <summary>Reads rs_compiledsnapshot and writes rs_trace with the json-formats.md settings.</summary>
public static class TraceJson
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        DateTimeZoneHandling = DateTimeZoneHandling.Utc,
        DateFormatString = "yyyy-MM-dd'T'HH:mm:ss'Z'",
        Formatting = Formatting.None,
    };

    // The compiled rule; null when the text is empty or not valid.
    public static CompiledRule? ReadSnapshot(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        try
        {
            return JsonConvert.DeserializeObject<CompiledRule>(text!, Settings);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    // The trace as JSON under maxLength: actions are cut from the end, then the condition tree is
    // dropped, and truncated is set (json-formats.md §4).
    public static string Write(ExecutionTrace trace, int maxLength)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(trace, nameof(trace));
        var text = JsonConvert.SerializeObject(trace, Settings);
        while (text.Length > maxLength && (trace.Actions.Count > 0 || trace.Conditions != null))
        {
            trace.Truncated = true;
            if (trace.Actions.Count > 0)
            {
                trace.Actions.RemoveAt(trace.Actions.Count - 1);
            }
            else
            {
                trace.Conditions = null;
            }

            text = JsonConvert.SerializeObject(trace, Settings);
        }

        return text;
    }
}
