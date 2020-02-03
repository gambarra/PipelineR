using System;

namespace PipelineR.Interface
{
    public interface ICondition<TContext> where TContext : BaseContext
    {
        Func<TContext, bool> When();
    }
}