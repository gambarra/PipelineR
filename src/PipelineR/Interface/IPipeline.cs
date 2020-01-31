namespace PipelineR.Interface
{
    public interface IPipeline<TContext, in TRequest> where TContext : BaseContext
    {
        RequestHandlerResult Execute(TRequest request);
    }
}