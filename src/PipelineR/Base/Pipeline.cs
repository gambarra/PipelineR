using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PipelineR.Interface;
using System;
using System.Linq;

namespace PipelineR.Base
{
    public class Pipeline<TContext, TRequest> : IPipeline<TContext, TRequest> where TContext : BaseContext
    {
        private readonly IServiceProvider _serviceProvider;

        private IStepHandler<TContext> _finallyStepHandler;
        private IStepHandler<TContext> _stepHandler;
        private IValidator<TRequest> _validator;

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

        public Pipeline<TContext, TRequest> AddValidator(IValidator<TRequest> validator)
        {
            _validator = validator;
            return this;
        }

        public Pipeline<TContext, TRequest> AddValidator<TValidator>()
        {
            var validator = (IValidator<TRequest>)_serviceProvider.GetService<TValidator>();
            return this.AddValidator(validator);
        }

        public Pipeline<TContext, TRequest> AddFinally(IStepHandler<TContext> stepHandler)
        {
            _finallyStepHandler = stepHandler;
            return this;
        }
        public Pipeline<TContext, TRequest> AddFinally<TStepHandler>()
        {
            var stepHandler = (IStepHandler<TContext>)_serviceProvider.GetService<TStepHandler>();
            return this.AddFinally(stepHandler);
        }

        public RequestHandlerResult Execute(TRequest request)
        {
            if (this._stepHandler is null)
                throw new ArgumentNullException("No started handlers");

            if (this._validator != null)
            {
                var validateResult = this._validator.Validate(request);

                if (validateResult.IsValid == false)
                {
                    var errors = (validateResult.Errors.Select(p => new ErrorResult(p.ErrorMessage))).ToList();
                    return new RequestHandlerResult(errors, 412);
                }
            }

            this._stepHandler.Context.Request = request;

            var result = StepOrchestrator.ExecuteHandler(request, this._stepHandler);

            result = ExecuteFinallyHandler() ?? result;

            return result;
        }

        private static IStepHandler<TContext> GetLastRequestHandler(
            IStepHandler<TContext> requestHandler)
        {
            if (requestHandler.NextStep != null)
                return GetLastRequestHandler(requestHandler.NextStep);

            return requestHandler;
        }

        private RequestHandlerResult ExecuteFinallyHandler()
        {
            RequestHandlerResult result = null;

            if (this._finallyStepHandler != null)
                result = this._finallyStepHandler.HandleStep();

            return result;
        }
    }
}