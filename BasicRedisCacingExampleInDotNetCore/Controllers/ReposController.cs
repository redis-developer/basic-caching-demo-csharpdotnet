using BasicRedisCacingExampleInDotNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BasicRedisCacingExampleInDotNetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReposController : ControllerBase
    {
        private readonly ILogger<ReposController> logger;
        private readonly IDistributedCache distributedCache;

        public ReposController(ILogger<ReposController> logger, IDistributedCache distributedCache)
        {
            this.logger = logger;
            this.distributedCache = distributedCache;
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> Get(string username)
        {
            var timer = new Stopwatch();
            timer.Start();

            var cache = await distributedCache.GetStringAsync($"/repos/:{username}");
            if (string.IsNullOrEmpty(cache))
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Chrome");
                    using (HttpResponseMessage res = await client.GetAsync($"https://api.github.com/users/{username}"))
                    using (HttpContent content = res.Content)
                    {
                        string jsonStr = await content.ReadAsStringAsync();
                        var gitData = JsonConvert.DeserializeObject<GitResponseModel>(jsonStr);
                        var data = new ResponseModel { Repos = gitData.PublicRepos.ToString(), Username = username, Cached = true };
                        await distributedCache.SetStringAsync($"/repos/:{username}", JsonConvert.SerializeObject(data), new DistributedCacheEntryOptions() { AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(3) });
                        data.Cached = false;
                        cache = JsonConvert.SerializeObject(data);
                    }
                }
            }

            timer.Stop();
            TimeSpan timeTaken = timer.Elapsed;
            Response.Headers.Add("x-response-time", new Microsoft.Extensions.Primitives.StringValues(timeTaken.Seconds + "." + timeTaken.Milliseconds + "ms"));

            return Content(cache, "application/json");
        }
    }
}
