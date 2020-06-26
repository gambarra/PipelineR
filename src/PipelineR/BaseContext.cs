using System.Text.Json.Serialization;

namespace PipelineR
{
    [JsonConverter(typeof(ContextConverter))]
    public abstract class BaseContext
    {
        public BaseContext()
        {
            this.Id = this.ToString();
        }
        public string Id { get; set; }
        public object Request { get; set; }
    
        public RequestHandlerResult Response { get; set; }
        public string CurrentRequestHandleId { get; set; }

    }
}