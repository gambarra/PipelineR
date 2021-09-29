using Polly;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PipelineR
{
    public abstract class RecoveryHandler<TContext, TRequest> : IRecoveryHandler<TContext, TRequest>
        where TContext : BaseContext
    {
        protected RecoveryHandler(TContext context)
        {
            this.Context = context;
        }

        public Expression<Func<TContext, TRequest, bool>> Condition { get; set; }
        internal Expression<Func<TContext, TRequest, bool>> RequestCondition { get; set; }
        public Policy Policy { get; set; }

        public TContext Context { get; private set; }

        protected RequestHandlerResult Abort(string errorMessage, int statusCode)
            => this.Context.Response = new RequestHandlerResult(errorMessage, statusCode, false)
            .WithRequestHandlerId(this.RequestHandleId());

        protected RequestHandlerResult Abort(string errorMessage)
            => this.Context.Response = new RequestHandlerResult(errorMessage, 0, false)
              .WithRequestHandlerId(this.RequestHandleId());

        protected RequestHandlerResult Abort(object errorResult, int statusCode)
            => this.Context.Response = new RequestHandlerResult(errorResult, statusCode, false)
              .WithRequestHandlerId(this.RequestHandleId());

        protected RequestHandlerResult Abort(object errorResult)
            => this.Context.Response = new RequestHandlerResult(errorResult, 0, false)
              .WithRequestHandlerId(this.RequestHandleId());

        protected RequestHandlerResult Abort(ErrorResult errorResult, int statusCode)
            => this.Context.Response = new RequestHandlerResult(errorResult, statusCode)
              .WithRequestHandlerId(this.RequestHandleId());
        protected RequestHandlerResult Abort(ErrorResult errorResult)
            => this.Context.Response = new RequestHandlerResult(errorResult, 0)
              .WithRequestHandlerId(this.RequestHandleId());
        protected RequestHandlerResult Finish(object result, int statusCode)
            => this.Context.Response = new RequestHandlerResult(result, statusCode, true)
              .WithRequestHandlerId(this.RequestHandleId());
        protected RequestHandlerResult Finish(object result)
            => this.Context.Response = new RequestHandlerResult(result, 0, true)
              .WithRequestHandlerId(this.RequestHandleId());

        public RequestHandlerResult Next() => null;

        public abstract RequestHandlerResult HandleRecovery(TRequest request);

        internal RequestHandlerResult Execute(TRequest request)
        {
            RequestHandlerResult result = null;

            if (this.Policy != null)
            {
                this.Policy.Execute(() =>
                {
                    result = HandleRecovery(request);
                });
            }
            else
            {
                result = HandleRecovery(request);
            }

            return result;
        }

        public string RequestHandleId()
            => this.GetType().Name;

        public void UpdateContext(TContext context)
        {
            context.ConvertTo(this.Context);
        }
    }

    public interface IRecoveryHandler<TContext, TRequest> : IHandler<TContext, TRequest> where TContext : BaseContext
    {
        RequestHandlerResult HandleRecovery(TRequest request);
        string RequestHandleId();
    }
}