using System.Collections.Generic;
using System.Net;

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
            this.Errors = new List<ErrorResult> { errorResult };
            this._success = false;
            this.StatusCode = statusCode;
        }

        public RequestHandlerResult(ErrorResult errorResult, HttpStatusCode statusCode)
        {
            this.Errors = new List<ErrorResult> { errorResult };
            this._success = false;
            this.StatusCode = (int) statusCode;
        }

        public RequestHandlerResult(object result, int statusCode, bool isSuccessful)
        {
            this._result = result;
            this._success = isSuccessful;
            this.StatusCode = statusCode;
        }

        public RequestHandlerResult(object result, HttpStatusCode statusCode, bool isSuccessful)
        {
            this._result = result;
            this._success = isSuccessful;
            this.StatusCode = (int) statusCode;
        }

        public bool IsSuccess() => _success;

        public object Result() => _result;
    }
}