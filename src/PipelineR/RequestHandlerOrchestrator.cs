namespace PipelineR
{
    public static class RequestHandlerOrchestrator
    {

        public static RequestHandlerResult ExecuteHandler<TContext, TRequest>(TRequest request,
            RequestHandler<TContext, TRequest> requestHandler) where TContext : BaseContext
        {
            RequestHandlerResult result ;

            if (requestHandler.Condition != null)
            {
                result = requestHandler.Condition.IsSatisfied(requestHandler.Context, request)
                    ? requestHandler.Execute(request)
                    : ((RequestHandler<TContext, TRequest>)requestHandler).Next(request);
            }
            else
            {
                result = requestHandler.Execute(request);
            }
               
            
            return result;
        }
    }
}
