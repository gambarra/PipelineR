using System;

namespace PipelineR
{
    public interface IStepHandler<TPipelineContext> where TPipelineContext : class
    {
        Func<TPipelineContext, bool> Condition { get; set; }

        TPipelineContext Context { get; set; }

        IStepHandler<TPipelineContext> NextStep { get; set; }

        StepHandlerResult HandleStep();
    }
}