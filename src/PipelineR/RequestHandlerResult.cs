using System.Collections.Generic;
using System.Net;
using WebApi.Models.Response;

namespace PipelineR
{
    public class RequestHandlerResult
    {
        public RequestHandlerResult()
        {

        }
        private readonly bool Success;

        private readonly object ResultObject;

        public IReadOnlyCollection<ErrorResult> Errors { private set; get; }

        public int StatusCode { get;  set; }

        public void SetStatusCode(int statusCode) => this.StatusCode = statusCode;

        public RequestHandlerResult(IReadOnlyCollection<ErrorResult> errors, int statusCode)
        {
            this.Errors = errors;
            this.Success = false;
            this.StatusCode = statusCode;
        }

        public RequestHandlerResult(IReadOnlyCollection<ErrorResult> errors) : this(errors, 0)
        {
        }

        public RequestHandlerResult(ErrorResult errorResult, int statusCode)
        {
            this.Errors = new List<ErrorResult> { errorResult };
            this.Success = false;
            this.StatusCode = statusCode;
        }

        public RequestHandlerResult(ErrorResult errorResult, HttpStatusCode statusCode)
        {
            this.Errors = new List<ErrorResult> { errorResult };
            this.Success = false;
            this.StatusCode = (int)statusCode;
        }

        public RequestHandlerResult(object result, int statusCode, bool isSuccessful)
        {
            this.ResultObject = result;
            this.Success = isSuccessful;
            this.StatusCode = statusCode;
        }

        public RequestHandlerResult(object result, HttpStatusCode statusCode, bool isSuccessful)
        {
            this.ResultObject = result;
            this.Success = isSuccessful;
            this.StatusCode = (int)statusCode;
        }

        public RequestHandlerResult(ErrorItemResponse error, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            this.ResultObject = new ErrorsResponse { Errors = new List<ErrorItemResponse> { error } };
            this.Success = false;
            this.StatusCode = (int)statusCode;
        }

        public RequestHandlerResult(ErrorsResponse errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            this.ResultObject = errors;
            this.Success = false;
            this.StatusCode = (int)statusCode;
        }

        public RequestHandlerResult(ErrorItemResponse error, int statusCode = 400)
        {
            this.ResultObject = new ErrorsResponse { Errors = new List<ErrorItemResponse> { error } };
            this.Success = false;
            this.StatusCode = statusCode;
        }

        public RequestHandlerResult(ErrorsResponse errors, int statusCode = 400)
        {
            this.ResultObject = errors;
            this.Success = false;
            this.StatusCode = statusCode;
        }

        public bool IsSuccess() => this.Success;

        public object Result() => this.ResultObject;
    }
}