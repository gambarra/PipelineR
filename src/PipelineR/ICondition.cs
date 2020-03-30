using System;
using System.Linq.Expressions;

namespace PipelineR
{
    public interface ICondition<TContext> where TContext : BaseContext
    {
        Expression<Func<TContext, bool>> When();
    }
}