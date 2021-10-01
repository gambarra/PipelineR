using PipelineR.Test.Request;

namespace PipelineR.Test.RequestHandlers
{
    public class CreateUserApiThreeHandler : RequestHandler<ContextSample, SampleRequest>, ICreateUserApiThreeHandler
    {
        public CreateUserApiThreeHandler(ContextSample context) : base(context)
        {
        }

        public override RequestHandlerResult HandleRequest(SampleRequest request)
        {
            this.Context.CreateUserApiThreeHandlerWasExecuted = true;
            return this.Next();
        }
    }

    public interface ICreateUserApiThreeHandler : IRequestHandler<ContextSample, SampleRequest>
    { }
}
