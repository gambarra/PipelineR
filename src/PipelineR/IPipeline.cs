using FluentValidation;
using System;
using System.Linq.Expressions;

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
        IPipeline<TContext> When(Expression<Func<TContext, bool>> func);
        IPipeline<TContext> When<TCondition>();
        IPipeline<TContext> SetValue<TPropertie>(Expression<Func<TContext, TPropertie>> action, TPropertie value);
    }
}