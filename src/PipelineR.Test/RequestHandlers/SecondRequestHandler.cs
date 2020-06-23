using System;
using System.Collections.Generic;
using System.Text;
using PipelineR.Test.Request;

namespace PipelineR.Test.RequestHandlers
{
    public class SecondRequestHandler : RequestHandler<ContextSample, SampleRequest>
    {
        public SecondRequestHandler(ContextSample context) : base(context)
        {
        }

        public override RequestHandlerResult HandleRequest(SampleRequest request)
        {
            throw new NotImplementedException();
        } 
    }
}
