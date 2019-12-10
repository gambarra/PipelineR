using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineR.Sample.Pipeline.Handlers
{
   public  class CreateLoginRollbackHandler : RollbackHandler<UserContext, UserRequest>, ICreateLoginRollbackHandler
    {
        public CreateLoginRollbackHandler(UserContext context) : base(context)
        {
        }

        public override void HandleRollback(UserRequest request)
        {
           Console.WriteLine("Rollback CreateLoginRollbackHandler Executado");
        }
    }

    public interface ICreateLoginRollbackHandler : IRollbackHandler<UserContext, UserRequest>
    {

    }
}
