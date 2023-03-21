using PipelineR.Test.Request;
using System;

namespace PipelineR.Test.RequestHandlers
{
    internal class CreateUserApiThreeDotOneHandler: RequestHandler<ContextSample, SampleRequest>, ICreateUserApiOneHandler
    {
        public CreateUserApiThreeDotOneHandler(ContextSample context) : base(context)
        {
        }

        public override RequestHandlerResult HandleRequest(SampleRequest request)
        {
            throw new Exception("Exception inside RequestHandler");
        }
    }

    public interface ICreateUserApiThreeDotOneHandler : IRequestHandler<ContextSample, SampleRequest>
    { }
}
