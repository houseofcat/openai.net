using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Clients.Responses;

public class OpenAIModelsResponse
{
    public string Object { get; set; }

    [JsonPropertyName("data")]
    public List<OpenAIModelResponse> Models { get; set; }
}
