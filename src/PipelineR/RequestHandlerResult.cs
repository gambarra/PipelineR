using System.Collections.Generic;

namespace PipelineR
{
    public class RequestHandlerResult
    {
        private readonly bool _success;

        private readonly object _result;

        public IReadOnlyCollection<ErrorResult> Errors { private set; get; }


        public int StatusCode { get; private set; }

        public RequestHandlerResult(IReadOnlyCollection<ErrorResult> errors, int statusCode)
        {
            this.Errors = errors;
            this._success = false;
            this.StatusCode = statusCode;
        }
        public RequestHandlerResult(IReadOnlyCollection<ErrorResult> errors) : this(errors, 0)
        {
        }

        public RequestHandlerResult(ErrorResult errorResult, int statusCode)
        {
            this.Errors = new List<ErrorResult>() { errorResult };
            this._success = false;
            this.StatusCode = statusCode;
        }

        public RequestHandlerResult(object result, int statusCode, bool isSuccessful)
        {
            this._result = result;
            this._success = isSuccessful;
            this.StatusCode = statusCode;
        }


        public bool IsSuccess() => _success;

        public object Result() => _result;
    }
}