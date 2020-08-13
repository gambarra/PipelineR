using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineR
{
    public class PipelinePolicyException:Exception
    {
        public PipelinePolicyException(RequestHandlerResult result)
        {
            this.Result = result;
        }
        public RequestHandlerResult Result { get;  private set; }

    }
}
