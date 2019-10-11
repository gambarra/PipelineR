using System.Collections.Generic;

namespace PipeR
{
    public class RequestHandlerResult
    {
        public RequestHandlerResult(IReadOnlyCollection<string> errors)
        {
            this.Errors = errors;
            this._success = false;
        }

        public RequestHandlerResult(string error) : this(new List<string>() {error})
        {
        }

        public RequestHandlerResult(object result)
        {
            this._result = result;
        }

        private readonly bool _success = true;

        private readonly object _result;
        public IReadOnlyCollection<string> Errors { private set; get; }

        public bool IsSuccess() => _success;

        public object Result() => _result;
    }
}