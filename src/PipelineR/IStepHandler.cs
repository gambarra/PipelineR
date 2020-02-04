using System;

namespace PipelineR
{
    public interface IStepHandler<TContext> where TContext : class
    {
        Func<TContext, bool> Condition { get; set; }

        TContext Context { get; set; }

        IStepHandler<TContext> NextStep { get; set; }

        StepHandlerResult HandleStep();
    }
}