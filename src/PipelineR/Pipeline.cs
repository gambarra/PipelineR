using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PipelineR.DrawingGraph;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PipelineR
{
    public class Pipeline<TContext> : IPipeline<TContext> where TContext : BaseContext
    {
        private readonly IServiceProvider _serviceProvider;

        private IStepHandler<TContext> _finallyStepHandler;
        private IStepHandler<TContext> _stepHandler; 
        private IValidator _validator;
  
        public Pipeline()
        {
            this._serviceProvider = PipelineRAutoInject.ServiceProvider;
        }

        public IPipeline<TContext> AddFinally(IStepHandler<TContext> stepHandler)
        {
            _finallyStepHandler = stepHandler;
            return this;
        }

        public IPipeline<TContext> AddFinally<TStepHandler>()
        {
            var stepHandler = (IStepHandler<TContext>)_serviceProvider.GetService<TStepHandler>();
            return this.AddFinally(stepHandler);
        }

        public IPipeline<TContext> SetValue<TPropertie>(Expression<Func<TContext, TPropertie>> action, TPropertie value)
        {
            var expression = (MemberExpression)action.Body;
            PropertyInfo pi = typeof(TContext).GetProperty(expression.Member.Name);

            var lastStepHandler = GetLastStepHandler(this._stepHandler);

            lastStepHandler.AddVariable(pi, value);

            return this;
        }

        public IPipeline<TContext> AddStep(IStepHandler<TContext> stepHandler)
        {
            if (this._stepHandler == null)
                this._stepHandler = stepHandler;
            else
                GetLastStepHandler(this._stepHandler).NextStep = stepHandler;

            return this;
        }

        public IPipeline<TContext> AddStep<TStepHandler>()
        {
            var stepHandler = (IStepHandler<TContext>)this._serviceProvider.GetService<TStepHandler>();
            return this.AddStep(stepHandler);
        }

        public IPipeline<TContext> AddValidator<TRequest>(IValidator<TRequest> validator) where TRequest : class
        {
            _validator = validator;
            return this;
        }  

        public IPipeline<TContext> AddValidator<TRequest>() where TRequest : class
        {
            var validator = _serviceProvider.GetService<IValidator<TRequest>>();
            _validator = validator;
            return this;
        }
        
        public StepHandlerResult Execute<TRequest>(TRequest request) where TRequest : class
        {
            if (this._stepHandler is null)
                throw new ArgumentNullException("No started steps");

            if (this._validator != null)
            {
                var validateResult = this._validator.Validate(request);

                if (!validateResult.IsValid)
                {
                    var errors = (validateResult.Errors.Select(p => new ErrorResult(p.ErrorMessage))).ToList();
                    return new StepHandlerResult(errors, 412);
                }
            }

            this._stepHandler.Context.Request(request);

            var result = StepOrchestrator.ExecuteHandler(this._stepHandler);

            result = ExecuteFinallyHandler() ?? result;

            return result;
        }

        public IPipeline<TContext> When(Expression<Func<TContext, bool>> func)
        {
            var lastStepHandler = GetLastStepHandler(this._stepHandler);
            lastStepHandler.Condition = func.Compile();
            return this;
        }

        public IPipeline<TContext> When<TCondition>()
        {
            var instance = (ICondition<TContext>)this._serviceProvider.GetService<TCondition>();
            return When(instance.When());
        }

        private static IStepHandler<TContext> GetLastStepHandler(
            IStepHandler<TContext> requestHandler)
        {
            if (requestHandler.NextStep != null)
                return GetLastStepHandler(requestHandler.NextStep);

            return requestHandler;
        }

        private StepHandlerResult ExecuteFinallyHandler()
        {
            StepHandlerResult result = null;

            if (this._finallyStepHandler != null)
                result = this._finallyStepHandler.HandleStep();

            return result;
        }
    }
}