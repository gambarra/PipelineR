using PipelineR.DrawingGraph;

namespace PipelineR
{
    public class PipelineStarting<TContext> : IPipelineStarting<TContext> where TContext : BaseContext
    {
        public IPipeline<TContext> Start() => new Pipeline<TContext>();
        public IPipeline<TContext> Start(string diagramTitle, string diagramDescription) => new Pipeline<TContext>();
    }

    public class PipelineStartingDiagram<TContext> : IPipelineStarting<TContext> where TContext : BaseContext
    {
        public IPipeline<TContext> Start() => new PipelineDiagram<TContext>();
        public IPipeline<TContext> Start(string diagramTitle, string diagramDescription) => new PipelineDiagram<TContext>(diagramTitle, diagramDescription);
    }

    public interface IPipelineStarting<TContext> where TContext : BaseContext
    {
        IPipeline<TContext> Start();
        IPipeline<TContext> Start(string diagramTitle, string diagramDescription);
    }
}