using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Clients.Responses;

public class OpenAICompletionResponse : OpenAIBaseResponse
{
    public long Created { get; set; }

    [JsonPropertyName("model")]
    public string ModelId { get; set; }

    [JsonPropertyName("choices")]
    public List<OpenAIChoice> Choices { get; set; }

    public OpenAIUsage Usage { get; set; }
}
