using System;
using System.Linq.Expressions;

namespace PipelineR.Extensions
{
    public static class Conditional
    {
        public static bool IsSatisfied<TContext, TRequest>(this Expression<Func<TContext, bool>> condition, TContext context, TRequest request)
        {
            var compiledExpression = condition.Compile();
            return compiledExpression(context);
        }

        //public static IRequestHandler<TContext, TRequest> When<TContext, TRequest>(
        //    this IRequestHandler<TContext, TRequest> requestHandler, Expression<Func<TContext, TRequest, bool>> condition) where TContext : BaseContext
        //{
        //    ((RequestHandler<TContext, TRequest>)requestHandler).Condition = condition;
        //    return requestHandler;
        //}
    }
}