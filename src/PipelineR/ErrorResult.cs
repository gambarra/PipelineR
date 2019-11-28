﻿using Newtonsoft.Json;
using System;

namespace PipelineR
{
    public class ErrorResult
    {
        public ErrorResult(string source, string message,Exception exception)
        {
            this.Exception = exception;
            this.Source = source;
            this.Message = message;
        }
        public ErrorResult(string source, Exception exception):this(source,String.Empty,exception) {

        }
        public ErrorResult(string source, string message):this(source,message,null) {

        }
        public ErrorResult( string message):this(null, message, null) {

        }
        public ErrorResult( Exception exception):this(string.Empty,string.Empty,exception) {

        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }

    }
}
