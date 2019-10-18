namespace PipelineR
{
    public class Pipeline<TContext, TRequest> : IPipeline<TContext, TRequest> where TContext : BaseContext
    {
        private IRequestHandler<TContext, TRequest> _requestHandler;

        public static Pipeline<TContext, TRequest> Build()
        {
            return new Pipeline<TContext, TRequest>();
        }

        public Pipeline<TContext, TRequest> AddNext(IRequestHandler<TContext, TRequest> requestHandler)
        {
            if (this._requestHandler == null)
            {
                this._requestHandler = requestHandler;
            }
            else
            {
                this.GetLastRequestHandler(this._requestHandler).NextRequestHandler = requestHandler;
            }

            return this;
        }

        public RequestHandlerResult Execute(TRequest request)
        {
            this._requestHandler.Context.Request = request;
            return this._requestHandler.HandleRequest(request);
        }

        private IRequestHandler<TContext, TRequest> GetLastRequestHandler(
            IRequestHandler<TContext, TRequest> requestHandler)
        {
            if (requestHandler.NextRequestHandler != null)
                return GetLastRequestHandler(requestHandler.NextRequestHandler);
            
            return requestHandler;
        }
    }

    public interface IPipeline<TContext, in TRequest> where TContext : BaseContext
    {
        RequestHandlerResult Execute(TRequest request);
    }
}