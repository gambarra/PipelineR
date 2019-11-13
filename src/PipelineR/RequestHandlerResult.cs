using System.Collections.Generic;

namespace PipelineR
{
    public class RequestHandlerResult
    {
        private readonly bool _success = true;

        private readonly object _result;

        public IReadOnlyCollection<string> Errors { private set; get; }

        public int StatusCode { get; set; }

        public RequestHandlerResult(IReadOnlyCollection<string> errors, int statusCode = 0)
        {
            this.Errors = errors;
            this._success = false;
            this.StatusCode = statusCode;
        }

        public RequestHandlerResult(string error, int statusCode)
        {
            this.Errors = new List<string>() { error };
            this._success = false;
            this.StatusCode = statusCode;
        }

        public RequestHandlerResult(object result, int statusCode, bool isSuccessful = true)
        {
            this._result = result;
            this._success = isSuccessful;
            this.StatusCode = statusCode;
        }


        public bool IsSuccess() => _success;

        public object Result() => _result;
    }
}