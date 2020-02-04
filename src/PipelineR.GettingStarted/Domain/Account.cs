using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineR.GettingStarted.Domain
{
    public class Account
    {
        public int Id { get; set; }
        public long BalanceInCents { get; set; }
        public string OwnerName { get; set; }
    }
}
