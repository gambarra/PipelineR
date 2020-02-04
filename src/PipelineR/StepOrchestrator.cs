namespace PipelineR
{
    public static class StepOrchestrator
    {
        public static StepHandlerResult ExecuteHandler<TContext>(IStepHandler<TContext> stepHandler) where TContext : BaseContext
        {
            if (stepHandler.Condition is null)
                return stepHandler.HandleStep();
            else if (stepHandler.Condition.IsSatisfied(stepHandler.Context))
                return stepHandler.HandleStep();
            else
                return ((StepHandler<TContext>)stepHandler).Continue();
        }
    }
}