using PipelineR.GettingStarted.Domain;

namespace PipelineR.GettingStarted.Workflows.Bank
{
    public class BankContext : BaseContext
    {
        public Account Account { get; set; }
    }
}