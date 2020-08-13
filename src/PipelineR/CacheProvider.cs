using JsonNet.PrivateSettersContractResolvers;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Polly;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PipelineR
{
    public class CacheProvider : ICacheProvider
    {
        public CacheProvider(CacheSettings redisSettings, IDistributedCache distributedCache)
        {
            this.redisSettings = redisSettings;
            this._distributedCache = distributedCache;
        }

        private readonly CacheSettings redisSettings;

        private readonly IDistributedCache _distributedCache;

        private JsonSerializer _serializer;
        private JsonSerializer GetSerializer()
        {
            if (_serializer == null)
                _serializer = new JsonSerializer()
                {
                    ContractResolver = new PrivateSetterContractResolver(),
                    Converters = { new ContextConverter() }
                };

            return _serializer;
        }


        public async Task<bool> Add<T>(T obj, string key)
        {
            await this._distributedCache.RemoveAsync(key);

            var ttl = TimeSpan.FromMinutes(this.redisSettings.TTLInMinutes);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                this.GetSerializer().Serialize(writer, obj);

                var json = sb.ToString();

                await this._distributedCache.SetStringAsync(key, json, options);
            }

            return true;
        }

        public async Task<T> Get<T>(string key)
        {

            var result = await Policy
                .Handle<Exception>()
                .RetryAsync(3)
                .ExecuteAsync(async () => await this._distributedCache.GetStringAsync(key));


            if (string.IsNullOrWhiteSpace(result))
            {
                return default(T);
            }
            JsonReader jsonReader = new JsonTextReader(new StringReader(result));

            return GetSerializer().Deserialize<T>(jsonReader);
        }
    }

    public interface ICacheProvider
    {
        Task<T> Get<T>(string key);

        Task<bool> Add<T>(T obj, string key);

    }
}
