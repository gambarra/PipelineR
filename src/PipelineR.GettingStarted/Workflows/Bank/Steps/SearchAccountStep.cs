using PipelineR.GettingStarted.Models;
using PipelineR.GettingStarted.Repositories;

namespace PipelineR.GettingStarted.Workflows.Bank.Steps
{
    public class SearchAccountStep : StepHandler<BankContext>, ISearchAccountStep
    {
        private readonly IBankRepository _repository;

        public SearchAccountStep(BankContext ctx, IBankRepository repository) : base(ctx)
        {
            _repository = repository;
        }

        public override StepHandlerResult HandleStep()
        {
            var request = this.Context.Request<DepositModel>();
            var account = _repository.Get(request.AccountId);

            if (account == null)
                this.Context.Response = new StepHandlerResult("Account NotFound", 200, false);

            this.Context.Account = account;

            return this.Continue();
        }
    }

    public interface ISearchAccountStep : IStepHandler<BankContext>
    {
    }
}