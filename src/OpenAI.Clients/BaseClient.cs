using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
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

    #region Getting

    private static readonly string GetErrorMessage = "Error during HttpClient Get. Message: {0}";
    private static readonly string GetClientErrorMessage = "Error during Identity.HttpClient Get. Type: {0}\r\nMessage: {0}";
    private static readonly string GetClientSuccessMessage = "Success during Identity.HttpClient Get for ClientId {0}.";
    private static readonly string GetDeserializeErrorMessage = "Error during HttpClient Get. Failed to deserialize response body. Message: {0}";
    private static readonly string GetResponseBodyErrorMessage = "Error during HttpClient Get. Failed to read response body. Message: {0}";

    protected async Task<HttpResponseMessage> GetAsync(
        string route,
        string successMessage,
        string errorMessage,
        string accessToken = null)
    {
        using var client = GetHttpClient(accessToken);

        HttpResponseMessage response = null;
        try
        {
            response = await client.GetAsync(route);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, GetErrorMessage, ex.Message);
            return null;
        }

        if (response.IsSuccessStatusCode)
        { _logger.LogDebug(successMessage); }
        else
        { _logger.LogError(errorMessage, response.ReasonPhrase); }

        return response;
    }

    protected async Task<TOut> GetAsync<TOut>(
        string route,
        string successMessage,
        string errorMessage,
        string accessToken = null,
        JsonSerializerOptions? options = null) where TOut : class, new()
    {
        using var client = GetHttpClient(accessToken);

        HttpResponseMessage response = null;
        try
        {
            response = await client.GetAsync(route);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, GetErrorMessage, errorMessage);
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            var serverErrorMessage = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(serverErrorMessage))
            { _logger.LogError($"Error: {response.ReasonPhrase}\r\nMessage: {errorMessage}"); }
            else
            { _logger.LogError($"Error: {serverErrorMessage}\r\nMessage: {errorMessage}"); }

            return null;
        }

        _logger.LogInformation(successMessage);

        try
        {
            return await response.GetFromJsonAsync<TOut>(options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, GetDeserializeErrorMessage, errorMessage);
            return null;
        }
    }

    #endregion
}