using PipelineR.Test.Request;

namespace PipelineR.Test.RequestHandlers
{
    public class UpdateAccountHandler : RequestHandler<ContextSample, SampleRequest>, IUpdateAccountHandler
    {
        public UpdateAccountHandler(ContextSample context) : base(context)
        {
        }

        public override RequestHandlerResult HandleRequest(SampleRequest request)
        {
            this.Context.UpdateAccountHandlerWasExecuted = true;
            return this.Next();
        }
    }

    public interface IUpdateAccountHandler : IRequestHandler<ContextSample, SampleRequest>
    { }
}
