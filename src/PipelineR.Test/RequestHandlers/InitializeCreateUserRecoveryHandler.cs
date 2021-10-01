using PipelineR.Test.Request;

namespace PipelineR.Test.RequestHandlers
{
    public class InitializeCreateUserRecoveryHandler : RecoveryHandler<ContextSample, SampleRequest>, IInitializeCreateUserRecoveryHandler
    {
        public InitializeCreateUserRecoveryHandler(ContextSample context) : base(context)
        {
        }

        public override RequestHandlerResult HandleRecovery(SampleRequest request)
        {
            this.Context.InitializeCreateUserRecoveryHandlerWasExecuted = true;

            if (this.Context.InitializeCreateUserRecoveryHandlerShouldAbort)
            {
                return this.Abort("Aborted", 400);
            }

            return this.Next();
        }
    }

    public interface IInitializeCreateUserRecoveryHandler : IRecoveryHandler<ContextSample, SampleRequest>
    { }
}