using System;

namespace PipelineR
{
    public static class Conditional
    {
        public static bool IsSatisfied<TContext>(this Func<TContext, bool> condition, TContext context)
        {
            return condition.Invoke(context);
        }
    }
}