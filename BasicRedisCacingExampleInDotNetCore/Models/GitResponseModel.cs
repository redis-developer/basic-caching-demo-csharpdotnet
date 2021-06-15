using System.Text.Json.Serialization;

namespace BasicRedisCacingExampleInDotNetCore.Models
{
    public class GitResponseModel
    {

        [JsonPropertyName("public_repos")]
        public int PublicRepos { get; set; }
    }
}
