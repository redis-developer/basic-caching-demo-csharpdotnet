using BasicRedisCacingExampleInDotNetCore.Models;
using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace BasicRedisCacingExampleInDotNetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReposController : ControllerBase
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly HttpClient _client;

        public ReposController(IConnectionMultiplexer redis, HttpClient client)
        {
            _redis = redis;
            _client = client;
        }

        /// <summary>
        /// Gets the number of repos for a user/organization
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("{username}")]
        public async Task<IActionResult> GetRepoCount(string username)
        {
            var db = _redis.GetDatabase();
            var timer = Stopwatch.StartNew();

            //check if we've already seen that username recently
            var cache = await db.StringGetAsync($"repos:{username}");
                        
            if (string.IsNullOrEmpty(cache))
            {
                //Since we haven't seen this username recently, let's grab it from the github API
                var gitData = await _client.GetFromJsonAsync<GitResponseModel>($"users/{username}");
                var data = new ResponseModel { Repos = gitData.PublicRepos.ToString(), Username = username, Cached = true };
                await db.StringSetAsync($"repos:{username}", JsonSerializer.Serialize(data), expiry: TimeSpan.FromSeconds(60));
                data.Cached = false;
                cache = JsonSerializer.Serialize(data);
            }

            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            Response.Headers.Add("x-response-time", $"{timeTaken.Seconds}.{timeTaken.Milliseconds}ms");
            return Content(cache, "application/json");
        }
    }
}
