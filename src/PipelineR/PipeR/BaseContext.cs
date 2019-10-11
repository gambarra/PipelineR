namespace PipeR
{
    public abstract class BaseContext
    {
        
        public object Request { get; set; }
        public RequestHandlerResult Response { get; set; }
        
        public RequestHandlerResult Fail(string errorMessage) {
            return Response = new RequestHandlerResult(errorMessage);
        }
        public RequestHandlerResult Success(object result) {
            return Response = new RequestHandlerResult(result);
        }
    }
}