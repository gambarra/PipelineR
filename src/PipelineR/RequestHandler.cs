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
        public Policy<RequestHandlerResult> PolicyRequestHandler { get; set; }

        private Pipeline<TContext, TRequest> _pipeline;
        public TContext Context { get; private set; }

        private int _rollbackIndex;

        private TRequest _request;

        #endregion

        #region Exit Pipeline

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

        protected RequestHandlerResult Rollback(RequestHandlerResult result)
        {
            this.Context.Response = result;

            this._pipeline.ExecuteRollback(this._rollbackIndex, this._request);

            return result;
        }


        #endregion

        #region Methods
        public RequestHandlerResult Next() => Next(string.Empty);


        public RequestHandlerResult Next(string requestHandlerId)
        {
            var request = (TRequest)this.Context.Request;

            if (this.NextRequestHandler != null)
            {
                this.Context.Response = RequestHandlerOrchestrator.ExecuteHandler(request, (RequestHandler<TContext, TRequest>)this.NextRequestHandler, requestHandlerId);
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
            else if (this.PolicyRequestHandler != null)
            {
                result = this.PolicyRequestHandler.Execute(() =>
                {
                    if (this.Context.Response != null && this.Context.Response.IsSuccess()==false && this.Context.Response.RequestHandlerId != this.RequestHandleId())
                    {
                        throw new PipelinePolicyException(this.Context.Response);
                    }

                    return HandleRequest(request);
                });

                if (result.IsSuccess() == false)
                {
                    throw new PipelinePolicyException(this.Context.Response);
                }
            }
            else
            {
                result = HandleRequest(request);
            }

            return result;
        }

        public void AddRollbackIndex(int rollbackIndex) => this._rollbackIndex = rollbackIndex;

        public void AddPipeline(Pipeline<TContext, TRequest> pipeline) => this._pipeline = pipeline;

        public string RequestHandleId()
        {
            return this.GetType().Name;
        }

        public void UpdateContext(TContext context)
        {
            context.ConvertTo(this.Context);
        }

        #endregion

    }

    public interface IRequestHandler<TContext, TRequest> : IHandler<TContext, TRequest> where TContext : BaseContext
    {
        RequestHandlerResult HandleRequest(TRequest request);
        IRequestHandler<TContext, TRequest> NextRequestHandler { get; set; }
        string RequestHandleId();
        Policy<RequestHandlerResult> PolicyRequestHandler { set; get; }
    }


}