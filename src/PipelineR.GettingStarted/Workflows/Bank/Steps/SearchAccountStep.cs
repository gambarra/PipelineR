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
            var id = this.Parameters.Get<int>("Id");
            var account = _repository.Get(id);

            if (account == null)
                this.Context.Response = new StepHandlerResult(this.Parameters.Get<string>("UnsuccessMessage"), 412, false);

            this.Context.Account = account;

            return this.Continue();
        }
    }

    public interface ISearchAccountStep : IStepHandler<BankContext>
    {
    }
}