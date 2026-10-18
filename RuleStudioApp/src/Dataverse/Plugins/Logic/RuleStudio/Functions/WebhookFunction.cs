using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SharedContext.Dao;

namespace DataverseLogic.RuleStudio.Functions;

/// <summary>
/// Standard custom function: posts a payload to an https URL. Side effects, not safe to retry.
/// The URL and the Authorization header can come from environment variables, so they differ per
/// environment and secrets stay out of the rules.
/// </summary>
public class WebhookFunction(IUserDataverseAccessObjectService dao, IWebhookSender sender)
{
    private const int MaxBodyInMessage = 300;

    private readonly IUserDataverseAccessObjectService dao = dao;
    private readonly IWebhookSender sender = sender;

    // The endpoint is an https URL, or the schema name of an environment variable that holds one.
    // The authorization variable names an environment variable with the Authorization header value.
    // Success when the response is 2xx, with the HTTP status code as the value.
    public (bool Success, int Value, string Message) CallWebhook(string? endpoint, string? payload, string? authorizationVariable)
    {
        var target = endpoint?.Trim() ?? string.Empty;
        if (target.Length == 0)
            return (false, 0, "Url is required.");

        if (!target.Contains("://"))
        {
            var fromVariable = EnvironmentVariable(target);
            if (string.IsNullOrWhiteSpace(fromVariable))
                return (false, 0, $"Environment variable {target} has no value.");

            target = fromVariable!.Trim();
        }

        if (!Uri.TryCreate(target, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
            return (false, 0, "The URL must be an absolute https URL.");

        string? authorization = null;
        if (!string.IsNullOrWhiteSpace(authorizationVariable))
        {
            authorization = EnvironmentVariable(authorizationVariable!.Trim());
            if (string.IsNullOrWhiteSpace(authorization))
                return (false, 0, $"Environment variable {authorizationVariable} has no value.");
        }

        var body = payload ?? string.Empty;
        var trimmed = body.TrimStart();
        var contentType = trimmed.Length > 0 && (trimmed[0] == '{' || trimmed[0] == '[')
            ? "application/json"
            : "text/plain";

        var (status, response) = sender.Post(uri, body, contentType, authorization);
        var excerpt = response.Length > MaxBodyInMessage ? response.Remove(MaxBodyInMessage) + "…" : response;
        var message = excerpt.Length == 0 ? $"HTTP {status}" : $"HTTP {status}: {excerpt}";
        return (status >= 200 && status < 300, status, message);
    }

    // The current value of an environment variable, or its default value when it has none.
    private string? EnvironmentVariable(string schemaName)
    {
        var query = new QueryExpression("environmentvariabledefinition") { ColumnSet = new ColumnSet("defaultvalue"), TopCount = 1 };
        query.Criteria.AddCondition("schemaname", ConditionOperator.Equal, schemaName);
        var value = query.AddLink("environmentvariablevalue", "environmentvariabledefinitionid", "environmentvariabledefinitionid", JoinOperator.LeftOuter);
        value.EntityAlias = "v";
        value.Columns = new ColumnSet("value");
        var row = dao.RetrieveList<Entity>(query).FirstOrDefault();
        if (row == null)
            return null;

        var current = row.GetAttributeValue<AliasedValue>("v.value")?.Value as string;
        return string.IsNullOrWhiteSpace(current) ? row.GetAttributeValue<string>("defaultvalue") : current;
    }
}
