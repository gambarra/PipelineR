using System;

namespace PipelineR
{
    public interface ICondition<TContext> where TContext : BaseContext
    {
        Func<TContext, bool> When();
    }
}