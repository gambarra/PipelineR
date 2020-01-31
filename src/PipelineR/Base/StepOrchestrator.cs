using PipelineR.Extensions;
using PipelineR.Interface;

namespace PipelineR.Base
{
    public static class StepOrchestrator
    {
        public static RequestHandlerResult ExecuteHandler<TContext, TRequest>(TRequest request,
            IStepHandler<TContext> stepHandler) where TContext : BaseContext
        {
            RequestHandlerResult result = null;

            if (stepHandler.Condition != null)
            {
                result = stepHandler.Condition.IsSatisfied(stepHandler.Context, request)
                    ? stepHandler.HandleStep()
                    : ((RequestHandler<TContext, TRequest>)stepHandler).Next(request);
            }
            else
            {
                result = stepHandler.HandleStep();
            }

            return result;
        }
    }
}