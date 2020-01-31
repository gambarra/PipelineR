using PipelineR.Base;
using PipelineR.Interface;
using System;
using System.Linq.Expressions;

namespace PipelineR
{
    public abstract class StepHandler<TContext> : IStepHandler<TContext> where TContext : BaseContext
    {
        public Expression<Func<TContext, bool>> Condition { get; set; }
        public TContext Context { get; private set; }
        public IStepHandler<TContext> NextStep { get; set; }
        public abstract RequestHandlerResult HandleStep();

        protected StepHandler(TContext context)
        {
            Context = context;
        }

        public RequestHandlerResult Continue()
        {
            if (this.NextStep != null)
                this.Context.Response = StepOrchestrator.ExecuteHandler(this.Context.Request, this.NextStep);

            return this.Context.Response;
        }
    }
}