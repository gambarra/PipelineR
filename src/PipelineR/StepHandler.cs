using PipelineR.Base;
using PipelineR.Interface;
using System;

namespace PipelineR
{
    public abstract class StepHandler<TContext> : IStepHandler<TContext> where TContext : BaseContext
    {
        protected StepHandler(TContext context)
        {
            Context = context;
        }

        public Func<TContext, bool> Condition { get; set; }
        public TContext Context { get; private set; }
        public IStepHandler<TContext> NextStep { get; set; }

        public StepHandlerResult Continue()
        {
            if (this.NextStep != null)
                this.Context.Response = StepOrchestrator.ExecuteHandler(this.NextStep);

            return this.Context.Response;
        }

        public abstract StepHandlerResult HandleStep();
        protected StepHandlerResult Abort(string errorMessage, int statusCode)
            => this.Context.Response = new StepHandlerResult(errorMessage, statusCode, false);

        protected StepHandlerResult Abort(string errorMessage)
            => this.Context.Response = new StepHandlerResult(errorMessage, 0, false);

        protected StepHandlerResult Abort(object errorResult, int statusCode)
             => this.Context.Response = new StepHandlerResult(errorResult, statusCode, false);

        protected StepHandlerResult Abort(object errorResult)
            => this.Context.Response = new StepHandlerResult(errorResult, 0, false);

        protected StepHandlerResult Abort(ErrorResult errorResult, int statusCode)
            => this.Context.Response = new StepHandlerResult(errorResult, statusCode);

        protected StepHandlerResult Abort(ErrorResult errorResult)
            => this.Context.Response = new StepHandlerResult(errorResult, 0);

        protected StepHandlerResult Finish(object result, int statusCode)
            => this.Context.Response = new StepHandlerResult(result, statusCode, true);

        protected StepHandlerResult Finish(object result)
            => this.Context.Response = new StepHandlerResult(result, 0, true);
    }
}