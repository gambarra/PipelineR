using System;

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
            RequestHandlerResult result = null;

            requestHandler.Context.CurrentRequestHandleId = requestHandler.RequestHandleId();

            if (UseRequestHandlerId(requestHandlerId) &&
                requestHandler.Context.CurrentRequestHandleId.Equals(requestHandlerId, System.StringComparison.InvariantCultureIgnoreCase) == false)
            {
                
                if(requestHandler.RecoveryRequestHandler != null)
                {
                    var recoveryRequestHandler = ((RecoveryHandler<TContext, TRequest>)requestHandler.RecoveryRequestHandler);
                    if (recoveryRequestHandler.Condition != null)
                    {
                        if(recoveryRequestHandler.Condition.IsSatisfied(recoveryRequestHandler.Context, request))
                        {
                            result = recoveryRequestHandler.Execute(request);
                        }
                    }
                    else
                    {
                        result = recoveryRequestHandler.Execute(request);
                    }
                    
                    requestHandler.Context.CurrentRequestHandleId = requestHandler.RecoveryRequestHandler.RequestHandleId();
                }

                if (result == null)
                {
                    return requestHandler.Next(requestHandlerId);
                }
            }
           
            if(result == null)
            {
                if (requestHandler.Condition != null)
                {
                    result = requestHandler.Condition.IsSatisfied(requestHandler.Context, request)
                        ? requestHandler.Execute(request)
                        : requestHandler.Next();
                }
                else
                {
                    result = requestHandler.Execute(request);
                }
            }

            return result;
        }

        private static bool UseRequestHandlerId(string requestHandlerId) => string.IsNullOrWhiteSpace(requestHandlerId) == false;
    }
}
