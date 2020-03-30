using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace PipelineR.DrawingGraph
{
    public class PipelineDiagram<TContext> : IPipeline<TContext> where TContext : BaseContext
    {
        private readonly DrawDiagram _diagram;

        public PipelineDiagram(string title, string description)
        {
            this._diagram = PipelineRAutoInject.ServiceProvider.GetService<DrawDiagram>();
            this._diagram.Configure(this, title, description);
        }

        public PipelineDiagram()
        {
            this._diagram = PipelineRAutoInject.ServiceProvider.GetService<DrawDiagram>();
        }

        public IPipeline<TContext> AddFinally(IStepHandler<TContext> stepHandler)
        {
            this._diagram.AddFinnaly(this, stepHandler.GetType().Name);
            return this;
        }

        public IPipeline<TContext> AddFinally<TStepHandler>()
        {
            var stepHandler = (IStepHandler<TContext>)PipelineRAutoInject.ServiceProvider.GetService<TStepHandler>();
            this._diagram.AddFinnaly(this, stepHandler.GetType().Name);
            return this;
        }

        public IPipeline<TContext> AddStep(IStepHandler<TContext> stepHandler)
        {
            this._diagram.AddStep(this, stepHandler.GetType().Name);
            return this;
        }

        public IPipeline<TContext> AddStep<TStepHandler>()
        {
            var stepHandler = (IStepHandler<TContext>)PipelineRAutoInject.ServiceProvider.GetService<TStepHandler>();
            this._diagram.AddStep(this, stepHandler.GetType().Name);
            return this;
        }

        public IPipeline<TContext> AddValidator<TRequest>(FluentValidation.IValidator<TRequest> validator) where TRequest : class
        {
            this._diagram.AddValidator(this);
            return this;
        }

        public IPipeline<TContext> AddValidator<TRequest>() where TRequest : class
        {
            this._diagram.AddValidator(this);
            return this;
        }

        public StepHandlerResult Execute<TRequest>(TRequest request) where TRequest : class
        {
            this._diagram.AddEnd(this);
            return null;
        }

        public IPipeline<TContext> When(Expression<Func<TContext, bool>> func)
        {
            var expressionBody = ExpressionBody(func);
            this._diagram.AddCondition(this, expressionBody);
            return this;
        }

        private string ExpressionBody<T>(Expression<Func<T, bool>> exp)
        {
            string expBody = ((LambdaExpression)exp).Body.ToString();

            var paramName = exp.Parameters[0].Name;
            var paramTypeName = "[Context]"; //exp.Parameters[0].Type.Name;

            expBody = expBody.Replace(paramName + ".", paramTypeName + ".")
                         .Replace("AndAlso", "&&").Replace("OrElse", "||");

            return expBody;
        }

        public IPipeline<TContext> When<TCondition>()
        {
            var instance = (ICondition<TContext>)PipelineRAutoInject.ServiceProvider.GetService<TCondition>();
            var expressionBody = ExpressionBody(instance.When());
            this._diagram.AddCondition(this, expressionBody);
            return this;
        }

        public IPipeline<TContext> SetValue<TPropertie>(Expression<Func<TContext, TPropertie>> action, TPropertie value)
        {
            return this;
        }
    }
}