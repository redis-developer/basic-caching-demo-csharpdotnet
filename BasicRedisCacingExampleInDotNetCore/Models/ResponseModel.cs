using System.Text.Json.Serialization;

namespace BasicRedisCacingExampleInDotNetCore.Models
{
    public class ResponseModel
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("repos")]
        public string Repos { get; set; }

        [JsonPropertyName("cached")]
        public bool Cached { get; set; }
    }
}
