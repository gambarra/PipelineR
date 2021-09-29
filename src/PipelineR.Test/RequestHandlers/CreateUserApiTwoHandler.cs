using PipelineR.Test.Request;

namespace PipelineR.Test.RequestHandlers
{
    public class CreateUserApiTwoHandler : RequestHandler<ContextSample, SampleRequest>, ICreateUserApiTwoHandler
    {
        public CreateUserApiTwoHandler(ContextSample context) : base(context)
        {
        }

        public override RequestHandlerResult HandleRequest(SampleRequest request)
        {
            this.Context.CreateUserApiTwoHandlerWasExecuted = true;
            return this.Next();
        }
    }

    public interface ICreateUserApiTwoHandler : IRequestHandler<ContextSample, SampleRequest>
    { }
}
