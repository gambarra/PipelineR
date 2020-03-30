using PipelineR.GettingStarted.Repositories;

namespace PipelineR.GettingStarted.Workflows.Bank.Steps
{
    public class AddCreditAccountStep : StepHandler<BankContext>, IAddCreditAccountStep
    {
        private readonly IBankRepository _repository;

        public AddCreditAccountStep(BankContext ctx, IBankRepository repository) : base(ctx)
        {
            _repository = repository;
        }

        public override StepHandlerResult HandleStep()
        {
            return this.Continue();
        }
    }

    public interface IAddCreditAccountStep : IStepHandler<BankContext>
    {
    }
}