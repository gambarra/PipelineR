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

        private readonly PipelineDiagram _diagram;

        private IStepHandler<TContext> _finallyStepHandler;
        private IStepHandler<TContext> _stepHandler;
        private IValidator<object> _validator;


        //public Pipeline()
        //{
        //}

        private Pipeline()
        {
            this._serviceProvider = PipelineRAutoInject.ServiceProvider;
            this._diagram = _serviceProvider.GetService<PipelineDiagram>();
        }

        //public static Pipeline<TContext> Configure()
        //{
        //    return new Pipeline<TContext>();
        //}

        public static Pipeline<TContext> Start()
        {
            var pipeline = new Pipeline<TContext>();
            return pipeline;
        }

        public static Pipeline<TContext> Start(TContext context)
        {
            var pipeline = new Pipeline<TContext>();
            pipeline._stepHandler.Context = context;
            return pipeline;
        }

        public Pipeline<TContext> CreateDiagram()
        {
            _diagram.BuildDiagram(this);
            return this;
        }

        public Pipeline<TContext> AddFinally(IStepHandler<TContext> stepHandler)
        {
            _finallyStepHandler = stepHandler;
            return this;
        }

        public Pipeline<TContext> AddFinally<TStepHandler>()
        {
            var stepHandler = (IStepHandler<TContext>)_serviceProvider.GetService<TStepHandler>();
            return this.AddFinally(stepHandler);
        }

        public Pipeline<TContext> SetParameter(string key, object value)
        {
            var lastStepHandler = GetLastStepHandler(this._stepHandler);
            lastStepHandler.Parameters.Add(key, value);

            return this;
        }

        public Pipeline<TContext> SetValue<TPropertie>(Expression<Func<TContext, TPropertie>> action, TPropertie value)
        {
            var expression = (MemberExpression)action.Body;
            var name = expression.Member.Name;

            Type type = typeof(TContext);

            PropertyInfo pi = type.GetProperty(name);
            pi.SetValue(this._stepHandler.Context, value);

            return this;
        }

        public Pipeline<TContext> AddStep(IStepHandler<TContext> stepHandler)
        {
            if (this._stepHandler == null)
                this._stepHandler = stepHandler;
            else
                GetLastStepHandler(this._stepHandler).NextStep = stepHandler;

            this._diagram.AddStep(this, stepHandler.GetType().Name);
            return this;
        }

        public Pipeline<TContext> AddStep(ICondition<TContext> condition)
        {
            var lastStepHandler = GetLastStepHandler(this._stepHandler);
            lastStepHandler.Condition = condition.When();

            return this;
        }

        public Pipeline<TContext> AddStep<TStepHandler>()
        {
            var stepHandler = (IStepHandler<TContext>)this._serviceProvider.GetService<TStepHandler>();

            return this.AddStep(stepHandler);
        }

        public Pipeline<TContext> AddValidator<TRequest>(IValidator<TRequest> validator) where TRequest : class
        {
            _validator = (IValidator<object>)validator;
            return this;
        }

        public Pipeline<TContext> AddValidator<TRequest>() where TRequest : class
        {
            var validator = _serviceProvider.GetService<IValidator<TRequest>>();
            return this.AddValidator(validator);
        }

        public StepHandlerResult Execute<TRequest>(TRequest request) where TRequest : class
        {
            if (this._stepHandler is null)
                throw new ArgumentNullException("No started handlers");

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

        public Pipeline<TContext> When(Func<TContext, bool> func)
        {
            var lastStepHandler = GetLastStepHandler(this._stepHandler);
            lastStepHandler.Condition = func;
            return this;
        }
        public Pipeline<TContext> When<TCondition>()
        {
            var instance = (ICondition<TContext>)this._serviceProvider.GetService<TCondition>();
            var lastStepHandler = GetLastStepHandler(this._stepHandler);
            lastStepHandler.Condition = instance.When();
            return this;
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