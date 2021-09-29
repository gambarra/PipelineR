namespace PipelineR.Sample.Pipeline.Handlers
{
    public class InitializeCreateUserRecoveryStep : RecoveryHandler<UserContext, UserRequest>, IInitializeCreateUserRecoveryStep
    {
        public InitializeCreateUserRecoveryStep(UserContext context) : base(context)
        {
        }

        public override RequestHandlerResult HandleRecovery(UserRequest request)
        {
            this.Context.RecoveryWasExecuted = true;
            return this.Next();
        }
    }

    public interface IInitializeCreateUserRecoveryStep : IRecoveryHandler<UserContext, UserRequest>
    {

    }
}
