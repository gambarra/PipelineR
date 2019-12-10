using System;
using System.Linq.Expressions;
using Polly;

namespace PipelineR
{
    public abstract class RequestHandler<TContext, TRequest> : IRequestHandler<TContext, TRequest>
        where TContext : BaseContext
    {


        #region  Constructores
        protected RequestHandler(TContext context)
        {
            this.Context = context;
        }

        protected RequestHandler(TContext context, Expression<Func<TContext, TRequest, bool>> condition) : this(context)
        {
            this.Condition = condition;
        }

        #endregion

        #region  Properties
        public Expression<Func<TContext, TRequest, bool>> Condition { get; set; }
        public IRequestHandler<TContext, TRequest> NextRequestHandler { get; set; }
        public Policy Policy { get; set; }

        private Pipeline<TContext, TRequest> _pipeline;
        public TContext Context { get; private set; }

        private int _rollbackIndex;

        private TRequest _request;

        #endregion

        #region Exit Pipeline

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

        protected RequestHandlerResult Rollback(RequestHandlerResult result)
        {
            this.Context.Response = result;

            this._pipeline.ExecuteRollback(this._rollbackIndex, this._request);

            return result;
        }


        #endregion


        #region Methods
        public RequestHandlerResult Next(TRequest request)
        {
            if (this.NextRequestHandler != null)
            {
                this.Context.Response = RequestHandlerOrchestrator.ExecuteHandler(request, (RequestHandler<TContext, TRequest>)this.NextRequestHandler);
            }

            return this.Context.Response;
        }

        public abstract RequestHandlerResult HandleRequest(TRequest request);

        internal RequestHandlerResult Execute(TRequest request)
        {
            _request = request;
            RequestHandlerResult result = null;

            if (this.Policy != null)
            {
                this.Policy.Execute(() =>
                {
                    result = HandleRequest(request);
                });
            }
            else
            {
                result = HandleRequest(request);
            }

            return result;
        }

        public void AddRollbackIndex(int rollbackIndex) => this._rollbackIndex = rollbackIndex;

        public void AddPipeline(Pipeline<TContext, TRequest> pipeline) => this._pipeline = pipeline;

        #endregion

    }

    public interface IRequestHandler<TContext, TRequest> : IHandler<TContext, TRequest> where TContext : BaseContext
    {
        RequestHandlerResult HandleRequest(TRequest request);
        IRequestHandler<TContext, TRequest> NextRequestHandler { get; set; }
    
    }
}