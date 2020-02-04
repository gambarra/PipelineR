using System;

namespace PipelineR
{
    public abstract class BaseContext
    {
        private object _request;

        public StepHandlerResult Response { get; set; }

        public void Request<TRequest>(TRequest request) where TRequest : class
        {
            _request = request;
        }

        public TRequest Request<TRequest>() where TRequest : class
        {
            return (TRequest)_request;
        }
    }
}