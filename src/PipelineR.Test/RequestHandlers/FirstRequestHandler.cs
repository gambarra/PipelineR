using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using PipelineR.Test.Request;

namespace PipelineR.Test.RequestHandlers
{
    public class FirstRequestHandler:RequestHandler<ContextSample,SampleRequest>
    {
        public FirstRequestHandler(ContextSample context) : base(context)
        {
        }

        public FirstRequestHandler(ContextSample context, Expression<Func<ContextSample, SampleRequest, bool>> condition):base(context,condition)
        {
            
        }

        public override RequestHandlerResult HandleRequest(SampleRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
