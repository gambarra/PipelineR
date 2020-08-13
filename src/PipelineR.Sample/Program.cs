using System;
using System.Net;
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

         var response=   pipeline.Execute(new UserRequest()
            {
                Name = "Yuri sd iopipf",
                Age = 10,
                DocumentNumber = "125"
            });

            Console.WriteLine("");

            Console.ReadLine();
        }

        static ServiceProvider IOC()
        {
            serviceProvider = new ServiceCollection();

        
            serviceProvider.AddStackExchangeRedisCache(options => {
                options.Configuration = "localhost";
            });

            serviceProvider.AddPipelineRCache(new CacheSettings() { TTLInMinutes=10});

            serviceProvider.AddScoped<IPipeline<UserContext, UserRequest>>(provider =>
            {
                var pipeline = Pipeline<UserContext, UserRequest>
                    .Configure(provider)
                    .UseRecoveryRequestByHash()
                    .AddNext<ICreateUserRequestHandler>()
                        .WithPolicy(Policy.HandleResult<RequestHandlerResult>(p => p.StatusCode != (int)HttpStatusCode.Created).Retry(3))
                    .AddNext<ICreateLoginMark1Handler>()
                        .WithPolicy(Policy.HandleResult<RequestHandlerResult>(p => p.StatusCode != (int)HttpStatusCode.Created).Retry(3))
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
