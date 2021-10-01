using PipelineR.Test.Request;

namespace PipelineR.Test.RequestHandlers
{
    public class InitializeCreateUserHandler : RequestHandler<ContextSample, SampleRequest>, IInitializeCreateUserHandler
    {
        public InitializeCreateUserHandler(ContextSample context) : base(context)
        {
        }

        public override RequestHandlerResult HandleRequest(SampleRequest request)
        {
            this.Context.InitializeCreateUserHandlerWasExecuted = true;
            return this.Next();
        }
    }

    public interface IInitializeCreateUserHandler : IRequestHandler<ContextSample, SampleRequest>
    { }
}
