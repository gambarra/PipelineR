using Newtonsoft.Json;
using System;

namespace PipelineR
{
    public class ErrorResult
    {
        public ErrorResult(string source, string message,Exception exception, object result=null)
        {
            this.Exception = exception;
            this.Source = source;
            this.Message = message;
            this.Result = result;
        }
        public ErrorResult(string source, Exception exception):this(source,null,exception) {

        }
        public ErrorResult(string source, string message):this(source,message,null) {

        }
        public ErrorResult( string message):this(null, message, null) {

        }
        public ErrorResult( Exception exception):this(null, null, exception) {

        }
        public ErrorResult(string source, object result) : this(source, null,null,result)
        {

        }
        public ErrorResult( object result) : this(null, null, null, result)
        {

        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; private set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; private set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Exception Exception { get; private set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Result { get; set; }

    }
}
