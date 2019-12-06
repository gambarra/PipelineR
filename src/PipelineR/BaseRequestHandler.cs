using System;
using System.Linq.Expressions;

namespace PipelineR
{
    public abstract class BaseRequestHandler<TContext, TRequest> : IRequestHandler<TContext, TRequest>
        where TContext : BaseContext
    {
        public IRequestHandler<TContext, TRequest> NextRequestHandler { get; set; }
        public TContext Context { get; private set; }

        protected BaseRequestHandler(TContext context)
        {
            this.Context = context;
        }

        public RequestHandlerResult Next(TRequest request)
        {
            if (this.NextRequestHandler != null)
            {
                this.Context.Response= RequestHandlerOrchestrator.ExecuteHandler(request, this.NextRequestHandler);
            }

            return this.Context.Response;
        }

        protected RequestHandlerResult Abort(string errorMessage, int statusCode = 0)
            => this.Context.Response = new RequestHandlerResult(errorMessage, statusCode);

        protected RequestHandlerResult Abort(object errorMessage, int statusCode = 0)
             => this.Context.Response = new RequestHandlerResult(errorMessage, statusCode, false);

        protected RequestHandlerResult Abort(ErrorResult errorResult, int statusCode = 0)
            => this.Context.Response = new RequestHandlerResult(errorResult, statusCode);

        protected RequestHandlerResult Finish(object result, int statusCode = 0)
            => this.Context.Response = new RequestHandlerResult(result, statusCode);

        public Expression<Func<TContext, TRequest, bool>> Condition { get; set; }

        public abstract RequestHandlerResult HandleRequest(TRequest request);
    }

    public interface IRequestHandler<TContext, TRequest> where TContext : BaseContext
    {
        Expression<Func<TContext, TRequest, bool>> Condition { get; set; }
        RequestHandlerResult HandleRequest(TRequest request);
        IRequestHandler<TContext, TRequest> NextRequestHandler { get; set; }
        TContext Context { get; }
    }
}