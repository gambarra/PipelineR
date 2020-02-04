namespace PipelineR.GettingStarted.Models
{
    public class CreateAccountModel
    {
        public int Id { get; set; }
        public long BalanceInCents { get; set; }
        public string OwnerName { get; set; }
    }
}