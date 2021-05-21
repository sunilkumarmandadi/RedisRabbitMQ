using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Redis.RabbitMQ;
using Redis.Redis;

namespace Redis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SortCodeController : ControllerBase
    {
        IDistributedCache _cache;
        public SortCodeController(IDistributedCache cache)
        {
            _cache = cache;
        }
        [HttpGet]
        public async Task<string> GetSortCodeAsync(string controlNumber)
        {
            string cachedResult = await _cache.GetStringAsync(controlNumber);
            if (cachedResult != null)
            {
                Producer.Publish(cachedResult);
                return cachedResult;
            }
            else
                return "Package not found.";
        }
        [HttpPost]
        public async Task SaveSortCodesAsync(Dictionary<string,string> sortcodes)
        {
            foreach (KeyValuePair<string, string> pair in sortcodes)
            {
                await RedisDataAccess.SetCacheValueAsync(_cache, pair.Key, pair.Value, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
            }
        }
    }
}
