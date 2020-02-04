using PipelineR.GettingStarted.Models;

namespace PipelineR.GettingStarted.Workflows.Bank.Steps
{
    public class SearchAccountStep : StepHandler<BankContext>, ISearchAccountStep
    {
        public SearchAccountStep(BankContext ctx) : base(ctx)
        {
        }

        public override StepHandlerResult HandleStep()
        {
            this.Context.AccountId = this.Context.Request<DepositModel>().AccountId;

            this.Context.Response = new StepHandlerResult("Search", 200, false);

            return this.Continue();
        }
    }

    public interface ISearchAccountStep : IStepHandler<BankContext>
    {
    }
}