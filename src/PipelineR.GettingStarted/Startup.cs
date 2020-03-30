using Microsoft.Extensions.DependencyInjection;
using PipelineR.DrawingGraph;
using PipelineR.GettingStarted.Repositories;

namespace PipelineR.GettingStarted
{
    public static class Startup
    {
        public static void AddPipelines(IServiceCollection services)
        {
            Repositories(services);
        }

        private static void Repositories(IServiceCollection services)
        {
            services.AddScoped<IBankRepository, BankRepository>();
        }
    }
}