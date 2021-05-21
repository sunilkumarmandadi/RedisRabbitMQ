using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redis.Redis
{
    public static class RedisDataAccess
    {
        public static async Task<T> GetCacheValueAsync<T>(this IDistributedCache cache, string key)
        {
            string result = await cache.GetStringAsync(key);
            if (String.IsNullOrEmpty(result))
            {
                return default(T);
            }
            var deserializedObj = JsonConvert.DeserializeObject<T>(result);
            return deserializedObj;
        }

        public static async Task SetCacheValueAsync<T>(this IDistributedCache cache,
                                                       string recordId,
                                                       T data,
                                                       TimeSpan? absoluteExpiryTime = null,
                                                       TimeSpan? unusedExpiryTime = null)
        {
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();

            // Remove item from cache after duration
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = absoluteExpiryTime;

            // Remove item from cache if unsued for the duration
            cacheEntryOptions.SlidingExpiration = unusedExpiryTime ?? TimeSpan.FromSeconds(60);
            
            var jsonData = JsonConvert.SerializeObject(data);// JsonSerializer.Serialize(ss);

            await cache.SetStringAsync(recordId, jsonData,cacheEntryOptions);
        }
    }

}
