using PipelineR.GettingStarted.Models;
using System;
using System.Linq.Expressions;

namespace PipelineR.GettingStarted.Workflows.Bank.Steps
{
    public class DepositAccountStep : StepHandler<BankContext>, IDepositAccountStep
    {
        public DepositAccountStep(BankContext ctx) : base(ctx)
        {
        }

        public override StepHandlerResult HandleStep()
        {
            return this.Finish("Transfer with Success");
        }        
    }

    public interface IDepositAccountStep : IStepHandler<BankContext>
    {
    }
}