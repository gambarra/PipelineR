using FluentValidation;

namespace PipelineR.GettingStarted.Models.Validators
{
    public class DepositModelValidator : AbstractValidator<DepositModel>
    {
        public DepositModelValidator()
        {
            ValidateAmount();
        }

        private void ValidateAmount()
        {
            RuleFor(r => r.Amount)
                .GreaterThan(1)
                .WithMessage("The amount should be greater than 1");
        }
    }
}