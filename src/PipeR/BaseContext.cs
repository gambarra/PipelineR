namespace PipeR
{
    public abstract class BaseContext
    {
        public object Request { get; set; }
        public RequestHandlerResult Response { get; set; }
        
    }
}