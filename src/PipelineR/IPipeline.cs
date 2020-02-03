namespace PipelineR.Interface
{
    public interface IPipeline<TContext, in TRequest> where TContext : class
    {
        StepHandlerResult Execute(TRequest request);
    }
}