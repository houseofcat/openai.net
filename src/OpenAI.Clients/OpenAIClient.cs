using Microsoft.Extensions.Logging;
using OpenAI.Clients.Responses;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenAI.Clients;

public class OpenAIClient : BaseClient
{
    private readonly OpenAIClientOptions _options;

    public OpenAIClient(
        ILogger<OpenAIClient> logger,
        HttpClient client,
        OpenAIClientOptions options) : base(logger, client, options.HttpClientName)
    {
        _options = options;
    }

    public OpenAIClient(
        ILogger<OpenAIClient> logger,
        IHttpClientFactory factory,
        OpenAIClientOptions options) : base(logger, factory, options.HttpClientName)
    {
        _options = options;
    }

    private static readonly string _getEngineSuccess = "Successfully retrieved models from OpenAI.";
    private static readonly string _getEngineFailure = "Failed to retrieved models from OpenAI.";
    
    public async Task<ApiModelsResponse> GetModelsAsync()
    {
        return await GetAsync<ApiModelsResponse>(
            $"{_options.ApiVersion}/models",
            _getEngineSuccess,
            _getEngineFailure,
            _options.ApiKey);
        
    }
}