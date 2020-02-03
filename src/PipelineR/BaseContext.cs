namespace PipelineR
{
    public abstract class BaseContext
    {
        public object Request { get; set; }
        public StepHandlerResult Response { get; set; }
    }
}