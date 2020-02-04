using PipelineR.GettingStarted.Models;
using System;

namespace PipelineR.GettingStarted.Workflows.Bank.Steps
{
    public class DepositAccountStep : StepHandler<BankContext>, IDepositAccountStep, IDepositAccountCondition
    {
        public DepositAccountStep(BankContext ctx) : base(ctx)
        {
        }

        public override StepHandlerResult HandleStep()
        {
            return this.Finish("Transfer with Success");
        }

        public Func<BankContext, bool> When()
        {
            return new Func<BankContext, bool>(p =>
            {
                var request = p.Request<DepositModel>();
                return p.AccountId == 10 && request.Amount > 0;
                //return true;
            });
        }
    }

    public interface IDepositAccountStep : IStepHandler<BankContext>
    {
    }

    public interface IDepositAccountCondition : ICondition<BankContext>
    {
    }
}