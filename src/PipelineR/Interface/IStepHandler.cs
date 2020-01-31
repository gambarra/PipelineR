using System;
using System.Linq.Expressions;

namespace PipelineR.Interface
{
    public interface IStepHandler<TContext> where TContext : BaseContext
    {
        Expression<Func<TContext, bool>> Condition { get; set; }

        TContext Context { get; }

        IStepHandler<TContext> NextStep { get; set; }

        RequestHandlerResult HandleStep();
    }
}