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

            pipeline.Execute(new UserRequest());

            Console.ReadLine();
        }

        static ServiceProvider IOC()
        {
            serviceProvider = new ServiceCollection();

            serviceProvider.AddScoped<IPipeline<UserContext, UserRequest>>(provider =>
            {
                var pipeline = Pipeline<UserContext, UserRequest>
                    .Configure(provider)
                    .AddNext<ICreateUserRequestHandler>()
                        .When((context, request) => request.Name == "joão")
                        .WithPolicy(Policy.Handle<Exception>().Retry(3))
                        .Rollback<ICreateUserRollbackHandler>()
                        .When((context, request) =>context.CreateUserRequestHandlerSuccess==false)
                    .AddNext<ICreateLoginRequestHandler>()
                        .WithPolicy(Policy.Handle<Exception>().Retry(2))
                        .Rollback<ICreateLoginRollbackHandler>();

               return pipeline;
           });

            serviceProvider.AddScoped<ICreateUserRequestHandler, CreateUserRequestHandler>();
            serviceProvider.AddScoped<ICreateLoginRequestHandler, CreateLoginRequestHandler>();
            serviceProvider.AddScoped<ICreateLoginRollbackHandler, CreateLoginRollbackHandler>();
            serviceProvider.AddScoped<ICreateUserRollbackHandler, CreateUserRollbackHandler>();
            serviceProvider.AddScoped<UserContext>();

          return  serviceProvider.BuildServiceProvider();
        }
    }
}
