using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Clients.Responses;

public class ApiModelsResponse
{
    public string Object { get; set; }

    [JsonPropertyName("data")]
    public List<Model> Models { get; set; }
}
