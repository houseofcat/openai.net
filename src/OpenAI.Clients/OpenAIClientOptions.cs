using Polly;
using System;
using System.Net.Http;

namespace OpenAI.Clients;

public class OpenAIClientOptions
{
    public string HttpClientName { get; set; } = "OpenAI.Client";
    public string BaseUrl { get; set; } = "https://api.openai.com/";
    public string ApiVersion { get; set; } = "v1";
    public string ApiKey { get; set; }
    public string Organization { get; set; }
    public string EngineId { get; set; }

    public TimeSpan HttpClientTimeout { get; set; }
    public TimeSpan HttpHandlerLifetime { get; set; }

    public IAsyncPolicy<HttpResponseMessage> PollyRetryPolicy { get; set; } = PollyPolicies.GetRetryPolicy();
    public IAsyncPolicy<HttpResponseMessage> PollyCircuitBreakerPolicy { get; set; }
}