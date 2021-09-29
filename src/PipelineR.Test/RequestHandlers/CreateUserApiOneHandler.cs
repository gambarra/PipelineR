using PipelineR.Test.Request;

namespace PipelineR.Test.RequestHandlers
{
    public class CreateUserApiOneHandler : RequestHandler<ContextSample, SampleRequest>, ICreateUserApiOneHandler
    {
        public CreateUserApiOneHandler(ContextSample context) : base(context)
        {
        }

        public override RequestHandlerResult HandleRequest(SampleRequest request)
        {
            this.Context.CreateUserApiOneHandlerWasExecuted = true;
            return this.Next();
        }
    }

    public interface ICreateUserApiOneHandler : IRequestHandler<ContextSample, SampleRequest>
    { }
}
