using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicRedisCacingExampleInDotNetCore.Models
{
    public class ResponseModel
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("repos")]
        public string Repos { get; set; }

        [JsonProperty("cached")]
        public bool Cached { get; set; }
    }
}
