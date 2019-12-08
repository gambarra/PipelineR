using System;
using System.Linq.Expressions;

namespace PipelineR
{
    public abstract class RequestHandler<TContext, TRequest> : IRequestHandler<TContext, TRequest>
        where TContext : BaseContext
    {
        public IRequestHandler<TContext, TRequest> NextRequestHandler { get; set; }
        public TContext Context { get; private set; }

        protected RequestHandler(TContext context)
        {
            this.Context = context;
        }

        protected RequestHandler(TContext context, Expression<Func<TContext, TRequest, bool>> condition ):this(context)
        {
            this.Condition = condition;
        }

        public RequestHandlerResult Next(TRequest request)
        {
            if (this.NextRequestHandler != null)
            {
                this.Context.Response= RequestHandlerOrchestrator.ExecuteHandler(request, this.NextRequestHandler);
            }

            return this.Context.Response;
        }

        protected RequestHandlerResult Abort(string errorMessage, int statusCode )
            => this.Context.Response = new RequestHandlerResult(errorMessage, statusCode,false);

        protected RequestHandlerResult Abort(string errorMessage)
            => this.Context.Response = new RequestHandlerResult(errorMessage, 0,false);

        protected RequestHandlerResult Abort(object errorResult, int statusCode )
             => this.Context.Response = new RequestHandlerResult(errorResult, statusCode, false);

        protected RequestHandlerResult Abort(object errorResult)
            => this.Context.Response = new RequestHandlerResult(errorResult, 0, false);

        protected RequestHandlerResult Abort(ErrorResult errorResult, int statusCode )
            => this.Context.Response = new RequestHandlerResult(errorResult, statusCode);
        protected RequestHandlerResult Abort(ErrorResult errorResult)
            => this.Context.Response = new RequestHandlerResult(errorResult, 0);
        protected RequestHandlerResult Finish(object result, int statusCode )
            => this.Context.Response = new RequestHandlerResult(result, statusCode,true);
        protected RequestHandlerResult Finish(object result)
            => this.Context.Response = new RequestHandlerResult(result, 0,true);

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