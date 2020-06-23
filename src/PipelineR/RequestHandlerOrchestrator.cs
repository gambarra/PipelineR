namespace PipelineR
{
    public static class RequestHandlerOrchestrator
    {

        public static RequestHandlerResult ExecuteHandler<TContext, TRequest>(TRequest request,
           RequestHandler<TContext, TRequest> requestHandler) where TContext : BaseContext 
            => ExecuteHandler(request, requestHandler, string.Empty);

        public static RequestHandlerResult ExecuteHandler<TContext, TRequest>(TRequest request,
            RequestHandler<TContext, TRequest> requestHandler, string requestHandlerId) where TContext : BaseContext
        {
            RequestHandlerResult result ;

            requestHandler.Context.CurrentRequestHandleId = requestHandler.RequestHandleId();

            if (UseRequestHandlerId(requestHandlerId)  &&
                requestHandler.Context.CurrentRequestHandleId.Equals(requestHandlerId, System.StringComparison.InvariantCultureIgnoreCase) == false)
                return requestHandler.Next(requestHandlerId);

            if (requestHandler.Condition != null)
            {
                result = requestHandler.Condition.IsSatisfied(requestHandler.Context, request)
                    ? requestHandler.Execute(request)
                    : ((RequestHandler<TContext, TRequest>)requestHandler).Next();
            }
            else
            {
                result = requestHandler.Execute(request);
            }
                  
            return result;
        }

        private static bool UseRequestHandlerId(string requestHandlerId)=>  string.IsNullOrWhiteSpace(requestHandlerId)==false;
    }
}
