using Microsoft.Extensions.DependencyInjection;
using PipelineR.DrawingGraph;
using PipelineR.GettingStarted.Repositories;
using PipelineR.GettingStarted.Workflows.Bank;
using PipelineR.GettingStarted.Workflows.Bank.Steps;

namespace PipelineR.GettingStarted
{
    public static class Startup
    {
        public static void AddPipelines(IServiceCollection services)
        {
            services.AddSingleton(p => new PipelineDiagram());

            BankPipeline(services);
            Repositories(services);
        }

        public static void BankPipeline(IServiceCollection services)
        {
            //services.AddScoped(p => new BankContext());

            //services.AddScoped<ISearchAccountStep, SearchAccountStep>();

            //services.AddScoped<IDepositAccountStep, DepositAccountStep>();
            //services.AddScoped<IDepositAccountCondition, DepositAccountStep>();

            //services.AddScoped<ICreateAccountStep, CreateAccountStep>();

            //services.AddScoped<IBankPipelineBuilder, BankPipelineBuilder>();
        }

        private static void Repositories(IServiceCollection services)
        {
            services.AddScoped<IBankRepository, BankRepository>();
        }
    }
}