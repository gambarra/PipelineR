using PipelineR.Test.Request;

namespace PipelineR.Test.RequestHandlers
{
    public class UpdateAccountRecoveryHandler : RecoveryHandler<ContextSample, SampleRequest>, IUpdateAccountRecoveryHandler
    {
        public UpdateAccountRecoveryHandler(ContextSample context) : base(context)
        {
        }

        public override RequestHandlerResult HandleRecovery(SampleRequest request)
        {
            this.Context.UpdateAccountRecoveryHandlerWasExecuted = true;
            return this.Next();
        }
    }

    public interface IUpdateAccountRecoveryHandler : IRecoveryHandler<ContextSample, SampleRequest>
    { }
}
