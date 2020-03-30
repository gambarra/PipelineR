using AspNetScaffolding.Extensions.Mapper;
using PipelineR.GettingStarted.Domain;
using PipelineR.GettingStarted.Models;
using PipelineR.GettingStarted.Repositories;

namespace PipelineR.GettingStarted.Workflows.Bank.Steps
{
    public class CreateAccountStep : StepHandler<BankContext>, ICreateAccountStep
    {
        private readonly IBankRepository _repository;

        public CreateAccountStep(BankContext ctx, IBankRepository repository) : base(ctx)
        {
            _repository = repository;
        }

        public override StepHandlerResult HandleStep()
        {
            var request = this.Context.Request<CreateAccountModel>();
            var account = request.As<Account>();

            _repository.Insert(account);

            this.Context.Response = new StepHandlerResult("Created", 200, true);

            return this.Continue();
        }
    }

    public interface ICreateAccountStep
    {
    }
}