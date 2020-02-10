using Newtonsoft.Json;
using System;

namespace PipelineR
{
    public class ErrorResult
    {
        public ErrorResult(string source, string message, Exception exception, object result, string property)
        {
            this.Exception = exception;
            this.Source = source;
            this.Message = message;
            this.Result = result;
            this.Property = property;
        }
        
        public ErrorResult(string source, Exception exception) : this(source, null, exception, null, null) 
        {
        }
        
        public ErrorResult(string source, string message) : this(source, message, null, null, null) 
        {
        }

        public ErrorResult(string source, string message, string property) : this(source, message, null, null, property)
        {
        }

        public ErrorResult(string message) : this(null, message, null, null, null) 
        {
        }
        
        public ErrorResult(Exception exception) : this(null, null, exception, null, null) 
        {
        }

        public ErrorResult(string source, object result) : this(source, null, null, result, null)
        {
        }

        public ErrorResult(object result) : this(null, null, null, result, null)
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

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Property { get; set; }
    }
}
