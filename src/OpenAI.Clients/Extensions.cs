using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenAI.Clients;
using Polly;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Clients;

public static class Extensions
{
    public static void AddOpenAIClient(
        this IServiceCollection services,
        OpenAIClientOptions options)
    {
        services.AddHttpClient(
            options.HttpClientName,
            options.BaseUrl,
            options.HttpClientTimeout,
            options.HttpHandlerLifetime,
            options.PollyRetryPolicy,
            options.PollyCircuitBreakerPolicy);

        services.AddSingleton(
            s => new OpenAIClient(
                s.GetRequiredService<ILoggerFactory>().CreateLogger<OpenAIClient>(),
                s.GetRequiredService<IHttpClientFactory>(),
                options));
    }

    public static TimeSpan DefaultHttpClientTimeout { get; set; } = TimeSpan.FromSeconds(20);
    public static TimeSpan DefaultHttpHandlerLifetime { get; set; } = TimeSpan.FromMinutes(5);

    public static IHttpClientBuilder AddHttpClient(
        this IServiceCollection services,
        string clientName,
        string clientUrl,
        TimeSpan? httpClientTimeout = null,
        TimeSpan? httpHandlerLifetime = null,
        IAsyncPolicy<HttpResponseMessage> pollyRetryPolicy = null,
        IAsyncPolicy<HttpResponseMessage> pollyCircuitBreakerPolicy = null)
    {
        var httpClientBuilder = services
                .AddHttpClient(
                    clientName,
                    options =>
                    {
                        options.BaseAddress = new Uri(clientUrl);
                        options.Timeout = httpClientTimeout ?? DefaultHttpClientTimeout;
                    })
                .SetHandlerLifetime(httpHandlerLifetime ?? DefaultHttpHandlerLifetime);

        if (pollyRetryPolicy != null)
        { httpClientBuilder.AddPolicyHandler(pollyRetryPolicy); }
        if (pollyCircuitBreakerPolicy != null)
        { httpClientBuilder.AddPolicyHandler(pollyCircuitBreakerPolicy); }

        return httpClientBuilder;
    }
}

public static class HttpMessageExtensions
{
    public static JsonSerializerOptions DefaultJsonSerializerOptions = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<TOut> GetFromJsonAsync<TOut>(
        this HttpResponseMessage response,
        JsonSerializerOptions options = null)
    {
        return await System.Text.Json.JsonSerializer.DeserializeAsync<TOut>(
            await response.Content.ReadAsStreamAsync(),
            options ?? DefaultJsonSerializerOptions);
    }
}
