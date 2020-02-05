using System;
using System.Collections.Generic;

namespace PipelineR
{
    public interface IStepHandler<TContext> where TContext : class
    {
        Func<TContext, bool> Condition { get; set; }

        TContext Context { get; set; }

        IStepHandler<TContext> NextStep { get; set; }

        StepHandlerResult HandleStep();

        Dictionary<string, object> Parameters { get; set; }
    }
}