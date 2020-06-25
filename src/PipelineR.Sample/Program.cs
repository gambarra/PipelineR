using System;
using Microsoft.Extensions.DependencyInjection;
using PipelineR.Sample.Pipeline;
using PipelineR.Sample.Pipeline.Handlers;
using Polly;

namespace PipelineR.Sample
{
    class Program
    {
        private static ServiceCollection serviceProvider;

        static void Main(string[] args)
        {

       


            Console.WriteLine("Hello World!");
            var service = IOC();

            var pipeline = service
                .GetService<IPipeline<UserContext, UserRequest>>();

            pipeline.Execute(new UserRequest()
            {
                Name = "Yuri ",
                Age = 10,
                DocumentNumber = "125"
            });

            Console.ReadLine();
        }

        static ServiceProvider IOC()
        {
            serviceProvider = new ServiceCollection();

        
            serviceProvider.AddStackExchangeRedisCache(options => {
                options.Configuration = "localhost";
            });

            serviceProvider.AddPipelineRCache(new CacheSettings());

            serviceProvider.AddScoped<IPipeline<UserContext, UserRequest>>(provider =>
            {
                var pipeline = Pipeline<UserContext, UserRequest>
                    .Configure(provider)
                    .UseRecoveryRequestByHash()
                    .AddNext<ICreateUserRequestHandler>()
                    .AddNext<ICreateLoginRequestHandler>();
     

                return pipeline;
            });

            serviceProvider.AddScoped<ICreateUserRequestHandler, CreateUserRequestHandler>();
            serviceProvider.AddScoped<ICreateLoginRequestHandler, CreateLoginRequestHandler>();
            serviceProvider.AddScoped<ICreateLoginMark1Handler, CreateLoginMark1Handler>();
            serviceProvider.AddScoped<ICreateUserRollbackHandler, CreateUserRollbackHandler>();
            serviceProvider.AddScoped<UserContext>();

            return serviceProvider.BuildServiceProvider();
        }
    }
}
