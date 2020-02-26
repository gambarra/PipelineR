using FluentValidation;

namespace PipelineR
{
    public interface IPipeline<TContext> 
        where TContext : class 
    {
        StepHandlerResult Execute<TRequest>(TRequest request) where TRequest : class;
        IPipeline<TContext> AddStep(IStepHandler<TContext> stepHandler);
        IPipeline<TContext> AddStep<TStepHandler>();
        IPipeline<TContext> AddFinally(IStepHandler<TContext> stepHandler);
        IPipeline<TContext> AddFinally<TStepHandler>();
        IPipeline<TContext> AddValidator<TRequest>(IValidator<TRequest> validator) where TRequest : class;
        IPipeline<TContext> AddValidator<TRequest>() where TRequest : class;
    }
}