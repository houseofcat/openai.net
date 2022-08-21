using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Clients.Responses;

public class Model
{
    public string Id { get; set; }

    public string Object { get; set; }

    public bool Ready { get; set; }

    [JsonPropertyName("owned_by")]
    public string OwnerBy { get; set; }

    [JsonPropertyName("permission")]
    public List<ModelPermission> Permissions { get; set; }

    public string Root { get; set; }
    public string Parent { get; set; }
}
