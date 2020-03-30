using System;
using System.Collections.Generic;
using System.Reflection;

namespace PipelineR
{
    public abstract class StepHandler<TContext> : IStepHandler<TContext> where TContext : BaseContext
    {
        protected StepHandler(TContext context)
        {
            Context = context;
            _variables = new Dictionary<PropertyInfo, object>();
        }

        public Func<TContext, bool> Condition { get; set; }
        public TContext Context { get; set; }
        public IStepHandler<TContext> NextStep { get; set; }

        private readonly Dictionary<PropertyInfo, object> _variables;

        public StepHandlerResult Continue()
        {
            if (this.NextStep != null)
                this.Context.Response = StepOrchestrator.ExecuteHandler(this.NextStep);

            return this.Context.Response;
        }

        public void AddVariable(PropertyInfo propertyInfo, object value) => _variables.Add(propertyInfo, value);

        public void LoadVariables()
        {
            foreach (var propertyInfo in _variables.Keys)
                propertyInfo.SetValue(Context, _variables[propertyInfo]);
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
        {
            if (result.GetType() == typeof(StepHandlerResult))
                this.Context.Response = (StepHandlerResult)result;
            else
                this.Context.Response = new StepHandlerResult(result, statusCode, true);

            return this.Context.Response;
        }

        protected StepHandlerResult Finish(object result)
        {
            if (result.GetType() == typeof(StepHandlerResult))
                this.Context.Response = (StepHandlerResult)result;
            else
                this.Context.Response = new StepHandlerResult(result, 0, true);

            return this.Context.Response;
        }
    }
}