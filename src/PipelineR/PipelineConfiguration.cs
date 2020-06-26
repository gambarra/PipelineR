using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineR
{
    public static class PipelineConfiguration
    {
  
        public static void AddPipelineRCache( this IServiceCollection services, CacheSettings redisSettings)
        {
            var provider = services.BuildServiceProvider();

            var distributedCache = provider.GetService<IDistributedCache>();

            services.AddSingleton<ICacheProvider>(new CacheProvider(redisSettings,distributedCache));
        }
    }
}
