using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PipelineR.Sample.Pipeline.Handlers
{
   public  class CreateUserRollbackHandler: RollbackHandler<UserContext, UserRequest>, ICreateUserRollbackHandler
    {
        public CreateUserRollbackHandler(UserContext context) : base(context)
        {
        }

        public override void HandleRollback(UserRequest request)
        {
            Console.WriteLine("Rollback CreateUserRollbackHandler Executado");
        }
    }

   public interface ICreateUserRollbackHandler: IRollbackHandler<UserContext, UserRequest>
   {

   }
}
