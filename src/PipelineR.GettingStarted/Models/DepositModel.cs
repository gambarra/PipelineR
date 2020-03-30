using System;
using System.Collections.Generic;

namespace PipelineR.GettingStarted.Models
{
    public class DepositModel
    {
        public DepositModel(int amount, int accountId, int destinationAccountId)
        {
            Amount = amount;
            AccountId = accountId;
            DestinationAccountId = destinationAccountId;
        }

        public string Name { get; set; }
        public long Amount { get; set; }
        public int AccountId { get; set; }
        public Guid Identifier { get; set; }
        public DateTime CreatedOn { get; set; }
        public int DestinationAccountId { get; set; }
        public CreateAccountModel Teste { get; set; }
        public Nullable<int> TesteNullable { get; set; }
        public List<CreateAccountModel> Accounts { get; set; }
        public int[] Arrays { get; set; }
        public Tipos TipoConta { get; set; }
    }

    public enum Tipos
    {
        Corrente,
        Poupanca
    }
}