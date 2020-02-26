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
            //this._diagram.AddStep(this, stepHandler.GetType().Name);
            return this;
        }

        public IPipeline<TContext> AddValidator<TRequest>() where TRequest : class
        {
            //this._diagram.AddStep(this, stepHandler.GetType().Name);
            return this;
        }

        public StepHandlerResult Execute<TRequest>(TRequest request) where TRequest : class
        {
            return null;
        }
    }
}