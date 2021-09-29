using PipelineR.Test.Request;

namespace PipelineR.Test.RequestHandlers
{
    public class CreateUserApiFourRecoveryHandler : RecoveryHandler<ContextSample, SampleRequest>, ICreateUserApiFourRecoveryHandler
    {
        public CreateUserApiFourRecoveryHandler(ContextSample context) : base(context)
        {
        }

        public override RequestHandlerResult HandleRecovery(SampleRequest request)
        {
            this.Context.CreateUserApiFourRecoveryHandlerWasExecuted = true;
            return this.Next();
        }
    }

    public interface ICreateUserApiFourRecoveryHandler : IRecoveryHandler<ContextSample, SampleRequest>
    { }
}
