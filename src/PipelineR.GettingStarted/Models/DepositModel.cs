namespace PipelineR.GettingStarted.Models
{
    public class DepositModel
    {
        public DepositModel(int amount, int accountId)
        {
            Amount = amount;
            AccountId = accountId;
        }

        public int Amount { get; set; }
        public int AccountId { get; set; }
    }
}