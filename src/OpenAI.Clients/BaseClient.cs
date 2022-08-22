using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Clients;

public class BaseClient
{
    protected readonly ILogger _logger;
    protected readonly IHttpClientFactory _httpClientFactory;
    protected HttpClient _client;
    protected readonly IMemoryCache _memoryCache;
    protected readonly string _clientName;
    private static readonly string AuthScheme = "Bearer";

    public BaseClient(
        ILogger logger,
        HttpClient client,
        string clientName)
    {
        _logger = logger;
        _client = client;
        _clientName = clientName;

        _memoryCache = new MemoryCache(
            new MemoryCacheOptions
            {
                ExpirationScanFrequency = TimeSpan.FromMinutes(6),
            });
    }

    public BaseClient(
        ILogger logger,
        IHttpClientFactory factory,
        string clientName)
    {
        _logger = logger;
        _httpClientFactory = factory;
        _clientName = clientName;

        _memoryCache = new MemoryCache(
            new MemoryCacheOptions
            {
                ExpirationScanFrequency = TimeSpan.FromMinutes(6),
            });
    }

    private static readonly string UserAgentKey = "User-Agent";
    public string UserAgent = "houseofcat/OpenAI.Net";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HttpClient GetHttpClient(string accessToken)
    {
        var client = _httpClientFactory?.CreateClient(_clientName) ?? _client;

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme, accessToken);
        }

        client.DefaultRequestHeaders.Add(UserAgentKey, UserAgent);

        return client;
    }

    private static readonly string _getErrorMessage = "Error during HttpClient Get. Message: {0}";
    private static readonly string _getDeserializeErrorMessage = "Error during HttpClient Get. Failed to deserialize response body. Message: {0}";
    private static readonly string _httpErrorFormat = "Error: {0}\nMessage: {1}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected async Task<TOut> GetAsync<TOut>(
        string route,
        string successMessage,
        string errorMessage,
        string accessToken = null,
        JsonSerializerOptions? options = null,
        CancellationToken token = default) where TOut : class, new()
    {
        using var client = GetHttpClient(accessToken);

        HttpResponseMessage response = null;
        try
        {
            response = await client.GetAsync(route, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, _getErrorMessage, errorMessage);
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            var serverErrorMessage = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(serverErrorMessage))
            { _logger.LogError(_httpErrorFormat, response.ReasonPhrase, errorMessage); }
            else
            { _logger.LogError(_httpErrorFormat, serverErrorMessage, errorMessage); }

            return null;
        }

        _logger.LogInformation(successMessage);

        try
        {
            return await response.GetFromJsonAsync<TOut>(options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, _getDeserializeErrorMessage, errorMessage);
            return null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected async Task<TOut> PostAsync<TIn, TOut>(
        TIn request,
        string route,
        string successMessage,
        string errorMessage,
        string accessToken = null,
        JsonSerializerOptions? options = null,
        CancellationToken token = default) where TOut : class, new()
    {
        using var client = GetHttpClient(accessToken);

        HttpResponseMessage response = null;
        try
        {
            response = await client.PostAsJsonAsync(
                route,
                request,
                options ?? HttpMessageExtensions.DefaultJsonSerializerOptions,
                token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, _getErrorMessage, errorMessage);
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            var serverErrorMessage = await response.Content.ReadAsStringAsync(token);

            if (string.IsNullOrWhiteSpace(serverErrorMessage))
            { _logger.LogError(_httpErrorFormat, response.ReasonPhrase, errorMessage); }
            else
            { _logger.LogError(_httpErrorFormat, serverErrorMessage, errorMessage); }

            return null;
        }

        _logger.LogInformation(successMessage);

        try
        {
            return await response.GetFromJsonAsync<TOut>(options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, _getDeserializeErrorMessage, errorMessage);
            return null;
        }
    }
}