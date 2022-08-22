using Microsoft.Extensions.Logging;
using OpenAI.Clients.Requests;
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

    private static readonly string _getModelsSuccess = "Successfully retrieved models from OpenAI.";
    private static readonly string _getModelsFailure = "Failed to retrieved models from OpenAI.";

    public async Task<OpenAIModelsResponse> GetModelsAsync()
    {
        return await GetAsync<OpenAIModelsResponse>(
            $"{_options.ApiVersion}/models",
            _getModelsSuccess,
            _getModelsFailure,
            _options.ApiKey);

    }

    private static readonly string _getModelSuccess = "Successfully retrieved model {0} from OpenAI.";
    private static readonly string _getModelFailure = "Failed to retrieved model {0} from OpenAI.";

    public async Task<OpenAIModelResponse> GetModelAsync(string modelId)
    {
        return await GetAsync<OpenAIModelResponse>(
            $"{_options.ApiVersion}/models/{modelId}",
            string.Format(_getModelSuccess, modelId),
            string.Format(_getModelFailure, modelId),
            _options.ApiKey);

    }

    private static readonly string _postCompleteSuccess = "Successfully created Completion from OpenAI.";
    private static readonly string _postCompleteFailure = "Failed to create Completion from OpenAI.";

    public async Task<OpenAICompletionResponse> PostCompletionAsync(OpenAICompletionRequest request)
    {
        return await PostAsync<OpenAICompletionRequest, OpenAICompletionResponse>(
            request,
            $"{_options.ApiVersion}/completions",
            _postCompleteSuccess,
            _postCompleteFailure,
            _options.ApiKey);

    }
}