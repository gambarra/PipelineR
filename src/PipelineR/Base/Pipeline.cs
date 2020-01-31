using Microsoft.Extensions.DependencyInjection;
using PipelineR.Interface;
using System;

namespace PipelineR.Base
{
    public class Pipeline<TContext, TRequest> : IPipeline<TContext, TRequest> where TContext : BaseContext
    {
        private readonly IServiceProvider _serviceProvider;

        //private IStepHandler<TContext> _finallyStepHandler;
        private IStepHandler<TContext> _stepHandler;

        public Pipeline()
        {
        }

        private Pipeline(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public static Pipeline<TContext, TRequest> Configure()
        {
            return new Pipeline<TContext, TRequest>();
        }

        public static Pipeline<TContext, TRequest> Configure(IServiceProvider serviceProvider)
        {
            return new Pipeline<TContext, TRequest>(serviceProvider);
        }

        public Pipeline<TContext, TRequest> AddNext(IStepHandler<TContext> stepHandler)
        {
            if (this._stepHandler == null)
                this._stepHandler = stepHandler;
            else
                GetLastRequestHandler(this._stepHandler).NextStep = stepHandler;

            return this;
        }

        public Pipeline<TContext, TRequest> AddNext<TStepHandler>()
        {
            var stepHandler = (IStepHandler<TContext>)this._serviceProvider.GetService<TStepHandler>();
            return this.AddNext(stepHandler);
        }

        public RequestHandlerResult Execute(TRequest request)
        {
            if (this._stepHandler is null)
                throw new ArgumentNullException("No started handlers");

            this._stepHandler.Context.Request = request;

            var result = StepOrchestrator.ExecuteHandler(request, this._stepHandler);

            return result;
        }

        //public Pipeline<TContext, TRequest> AddNext<TStepHandler>() => AddNext<TStepHandler>();

        private static IStepHandler<TContext> GetLastRequestHandler(
            IStepHandler<TContext> requestHandler)
        {
            if (requestHandler.NextStep != null)
                return GetLastRequestHandler(requestHandler.NextStep);

            return requestHandler;
        }
    }
}