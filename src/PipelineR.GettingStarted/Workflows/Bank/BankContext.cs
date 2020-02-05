using PipelineR.GettingStarted.Domain;

namespace PipelineR.GettingStarted.Workflows.Bank
{
    public class BankContext : BaseContext
    {
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}