namespace DataverseLogic.RuleStudio.Functions;

/// <summary>Sends an HTTP POST; the default implementation is <see cref="HttpWebhookSender"/>.</summary>
public interface IWebhookSender
{
    (int Status, string Body) Post(Uri endpoint, string payload, string contentType, string? authorization);
}
