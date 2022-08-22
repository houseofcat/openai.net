using System.Text.Json.Serialization;

namespace OpenAI.Clients.Responses
{
    public class OpenAIChoice
    {
        public string Text { get; set; }
        public int Index { get; set; }

        public int? LogProbs { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }
    }
}