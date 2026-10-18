#if NETFRAMEWORK
using System.Net.Http;
#endif
using System.Net.Http.Headers;
using System.Text;

namespace DataverseLogic.RuleStudio.Functions;

/// <summary>Posts with one shared HttpClient. Dataverse stops a plug-in after 2 minutes, so a call gives up after 30 seconds.</summary>
public sealed class HttpWebhookSender : IWebhookSender
{
    private static readonly HttpClient Client = CreateClient();

    public (int Status, string Body) Post(Uri endpoint, string payload, string contentType, string? authorization)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = new StringContent(payload, Encoding.UTF8, contentType) };
        if (!string.IsNullOrWhiteSpace(authorization))
        {
            request.Headers.TryAddWithoutValidation("Authorization", authorization);
        }

#pragma warning disable CA2025 // The call completes synchronously before the request is disposed.
        using var response = Client.SendAsync(request).GetAwaiter().GetResult();
#pragma warning restore CA2025
        var body = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        return ((int)response.StatusCode, body);
    }

    private static HttpClient CreateClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RuleStudio", "1.0"));
        return client;
    }
}
