using Microsoft.Extensions.DependencyInjection;

namespace PipelineR.DrawingGraph
{
    public class PipelineDiagram<TContext> : IPipeline<TContext> where TContext : BaseContext
    {
        private readonly DrawDiagram _diagram;

        public PipelineDiagram()
        {
            this._diagram = PipelineRAutoInject.ServiceProvider.GetService<DrawDiagram>();
        }

        public IPipeline<TContext> AddStep(IStepHandler<TContext> stepHandler)
        {
            this._diagram.AddStep(this, stepHandler.GetType().Name);
            return this;
        }

        public IPipeline<TContext> AddStep<TStepHandler>()
        {
            this._diagram.AddStep(this, typeof(IStepHandler<TContext>).Name);
            return this;
        }

        public StepHandlerResult Execute<TRequest>(TRequest request) where TRequest : class
        {
            return null;
        }
    }
}