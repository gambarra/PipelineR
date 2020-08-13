using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineR
{
    public class PipelineSnapshot
    {
        public PipelineSnapshot()
        {

        }
        public PipelineSnapshot(bool success, string lastRequestHandlerId, BaseContext context)
        {
            this.CreatedAt = DateTime.UtcNow;
            this.Success = success;
            this.LastRequestHandlerId = lastRequestHandlerId;
            this.Context = context;

            if (this.Context?.Response != null && this.Context.Response.IsSuccess() == false)
            {
                this.Context.Response = null;
            }

        }
        public DateTime CreatedAt { get; set; }
        public bool Success { get; private set; }
        public string LastRequestHandlerId { get; private set; }
        public BaseContext Context { get; set; }
    }
}
