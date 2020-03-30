using System;
using System.Linq.Expressions;

namespace PipelineR.GettingStarted.Workflows.Bank.Condition
{
    public interface IDepositAccountCondition : ICondition<BankContext>
    {
    }

    public class DepositAccountCondition : IDepositAccountCondition
    {
        public Expression<Func<BankContext, bool>> When()
        {
            return p => p.Account != null;
        }
    }
}