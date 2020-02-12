using System;

namespace PipelineR.GettingStarted.Workflows.Bank.Condition
{
    public class CreateAccountCondition : ICreateAccountCondition
    {
        public Func<BankContext, bool> When()
        {
            throw new NotImplementedException();
        }
    }

    public interface ICreateAccountCondition : ICondition<BankContext>
    {
    }
}