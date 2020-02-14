using PipelineR.DrawingGraph;

namespace PipelineR
{
    public class PipelineStarting<TContext> : IPipelineStarting<TContext> where TContext : BaseContext
    {
        public IPipeline<TContext> Start() => new Pipeline<TContext>();
    }

    public class PipelineStartingDiagram<TContext> : IPipelineStarting<TContext> where TContext : BaseContext
    {
        public IPipeline<TContext> Start() => new PipelineDiagram<TContext>();
    }

    public interface IPipelineStarting<TContext> where TContext : BaseContext
    {
        IPipeline<TContext> Start();
    }
}