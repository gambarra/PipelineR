using FluentValidation;
using PipelineR.GettingStarted.Models;
using PipelineR.GettingStarted.Models.Validators;
using PipelineR.GettingStarted.Workflows.Bank.Condition;
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
                        .Start("Create Account", "Creating an bank account")
                        .AddStep<ISearchAccountStep>()
                            .SetValue(ctx => ctx.AccountId, model.Id)
                            .SetValue(ctx => ctx.MessageFailed, "User already exists.")
                        .AddStep<ICreateAccountStep>()
                            .When(ctx => ctx.Account == null)
                        .Execute(model);
        }

        public StepHandlerResult Deposit(DepositModel model)
        {
            return Pipeline
                        .Start("Deposit", "Making a bank deposit")
                        .AddValidator<DepositModel>()
                        .AddStep<ISearchAccountStep>()
                            .SetValue(ctx => ctx.AccountId, model.AccountId)
                            .SetValue(ctx => ctx.MessageFailed, "Origin account not exists.")
                        .AddStep<ISearchAccountStep>()
                            .SetValue(ctx => ctx.AccountId, model.DestinationAccountId)
                            .SetValue(ctx => ctx.MessageFailed, "Destination account not exists.")
                        .AddStep<IAddCreditAccountStep>()
                            .When<IDepositAccountCondition>()
                        .AddStep<ISearchAccountStep>()
                        //.AddFinally<IDepositAccountStep>()
                        .Execute(model);
        }
    }
    
    public interface IBankPipelineBuilder : IWorkflow<BankContext>
    {
        StepHandlerResult CreateAccount(CreateAccountModel model);
        StepHandlerResult Deposit(DepositModel model);
    }

}