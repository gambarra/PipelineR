using PipelineR.GettingStarted.Models;
using PipelineR.GettingStarted.Workflows.Bank.Steps;

namespace PipelineR.GettingStarted.Workflows.Bank
{
    public class BankPipelineBuilder : IBankPipelineBuilder
    {
        public IPipelineStarting<BankContext> Pipeline { get; }

        public BankPipelineBuilder(IPipelineStarting<BankContext> pipeline)
        {
            Pipeline = pipeline;
        }

        public StepHandlerResult CreateAccount(CreateAccountModel model)
        {
            return Pipeline
                        .Start()
                        .AddStep<ISearchAccountStep>()
                            //.SetParameter("Id", model.Id)
                            //.SetParameter("UnsuccessMessage", "Account not exist.")
                        .AddStep<ICreateAccountStep>()
                            //.When(ctx => ctx.Account == null)
                        //.CreateDiagram()
                        .Execute(model);
        }

        public StepHandlerResult Deposit(DepositModel model)
        {
            return Pipeline
                        .Start()
                            //.SetValue(ctx => ctx.AccountId, model.AccountId)
                        .AddStep<ISearchAccountStep>()
                            //.SetValue(ctx => ctx.AccountId, model.AccountId)
                            //.SetParameter("UnsuccessMessage", "AccountId not exist")
                        .AddStep<ISearchAccountStep>()
                            //.SetValue(ctx => ctx.AccountId, model.DestinationAccountId)
                            //.SetParameter("UnsuccessMessage", "DestinationAccountId not exist")
                        .AddStep<IDepositAccountStep>()
                            //.When<IDepositAccountCondition>()
                        .Execute(model);
        }
    }
    
    public interface IBankPipelineBuilder : IWorkflow<BankContext>
    {
        StepHandlerResult CreateAccount(CreateAccountModel model);
        StepHandlerResult Deposit(DepositModel model);
    }

}