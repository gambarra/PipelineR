using System;
using System.Linq.Expressions;

namespace PipelineR
{
    public static class ConditionalExtension
    {
        public static bool IsSatisfied<TContext, TRequest>(this Expression<Func<TContext, TRequest, bool>> condition, TContext context, TRequest request)
        {
            var compiledExpression = condition.Compile();
            return compiledExpression(context,request);
        }

        public static IRequestHandler<TContext, TRequest> When<TContext, TRequest>(
            this IRequestHandler<TContext, TRequest> requestHandler, Expression<Func<TContext, TRequest, bool>> condition) where TContext : BaseContext
        {
            ((BaseRequestHandler<TContext, TRequest>) requestHandler).Condition = condition;
            return requestHandler;
        }
   
    }
}
