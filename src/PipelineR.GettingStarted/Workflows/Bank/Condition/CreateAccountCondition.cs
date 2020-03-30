using System;
using System.Linq.Expressions;

namespace PipelineR.GettingStarted.Workflows.Bank.Condition
{
    public class CreateAccountCondition : ICreateAccountCondition
    {
        public Expression<Func<BankContext, bool>> When()
        {
            throw new NotImplementedException();
        }
    }

    public interface ICreateAccountCondition : ICondition<BankContext>
    {
    }
}