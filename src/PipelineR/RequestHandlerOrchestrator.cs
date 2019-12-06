namespace PipelineR
{
    public class RequestHandlerOrchestrator
    {

        public static RequestHandlerResult ExecuteHandler<TContext, TRequest>(TRequest request,
            IRequestHandler<TContext, TRequest> requestHandler) where TContext : BaseContext
        {
            RequestHandlerResult result = null;
            if (requestHandler.Condition != null)
            {
                result = requestHandler.Condition.IsSatisfied(requestHandler.Context, request)
                    ? requestHandler.HandleRequest(request)
                    : ((BaseRequestHandler<TContext, TRequest>)requestHandler).Next(request);
            }
            else
            {
                result = requestHandler.HandleRequest(request);
            }

            return result;
        }
    }
}
