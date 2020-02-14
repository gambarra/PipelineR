namespace PipelineR
{
    public interface IPipeline<TContext> 
        where TContext : class 
    {
        StepHandlerResult Execute<TRequest>(TRequest request) where TRequest : class;
        //IPipeline<TContext> AddStep(IStepHandler<TContext> stepHandler);
        IPipeline<TContext> AddStep<TStepHandler>();
    }

    //public interface IPipeline<TContext, TRequest>
    //where TContext : class
    //where TRequest : class
    //{
    //    StepHandlerResult Execute(TRequest request);
    //}
}