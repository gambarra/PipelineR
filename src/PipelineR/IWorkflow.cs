namespace PipelineR
{
    public interface IWorkflow<TContext> where TContext : BaseContext
    {
        IPipelineStarting<TContext> Pipeline { get; } 
    }
}