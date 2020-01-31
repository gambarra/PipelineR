using PipelineR.Base;
using PipelineR.Interface;
using System;
using System.Linq.Expressions;

namespace PipelineR
{
    public abstract class StepHandler<TContext> : IStepHandler<TContext> where TContext : BaseContext
    {
        protected StepHandler(TContext context)
        {
            Context = context;
        }

        public Expression<Func<TContext, bool>> Condition { get; set; }
        public TContext Context { get; private set; }
        public IStepHandler<TContext> NextStep { get; set; }

        public abstract RequestHandlerResult HandleStep();

        public RequestHandlerResult Continue()
        {
            if (this.NextStep != null)
                this.Context.Response = StepOrchestrator.ExecuteHandler(this.Context.Request, this.NextStep);

            return this.Context.Response;
        }

        protected RequestHandlerResult Abort(string errorMessage, int statusCode)
            => this.Context.Response = new RequestHandlerResult(errorMessage, statusCode, false);

        protected RequestHandlerResult Abort(string errorMessage)
            => this.Context.Response = new RequestHandlerResult(errorMessage, 0, false);

        protected RequestHandlerResult Abort(object errorResult, int statusCode)
             => this.Context.Response = new RequestHandlerResult(errorResult, statusCode, false);

        protected RequestHandlerResult Abort(object errorResult)
            => this.Context.Response = new RequestHandlerResult(errorResult, 0, false);

        protected RequestHandlerResult Abort(ErrorResult errorResult, int statusCode)
            => this.Context.Response = new RequestHandlerResult(errorResult, statusCode);

        protected RequestHandlerResult Abort(ErrorResult errorResult)
            => this.Context.Response = new RequestHandlerResult(errorResult, 0);

        protected RequestHandlerResult Finish(object result, int statusCode)
            => this.Context.Response = new RequestHandlerResult(result, statusCode, true);

        protected RequestHandlerResult Finish(object result)
            => this.Context.Response = new RequestHandlerResult(result, 0, true);
    }
}