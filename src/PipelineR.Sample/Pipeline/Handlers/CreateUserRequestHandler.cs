using System;
using System.Linq.Expressions;

namespace PipelineR.Sample.Pipeline.Handlers
{
    public class CreateUserRequestHandler:RequestHandler<UserContext,UserRequest>, ICreateUserRequestHandler
    {
        public static int retryCount = 0;
        public CreateUserRequestHandler(UserContext context) : base(context)
        {
        }


        public override RequestHandlerResult HandleRequest(UserRequest request)
        {
   
            this.Context.CreateUserRequestHandlerSuccess = true;
          
            return this.Next();
        }
    }

    public interface ICreateUserRequestHandler : IRequestHandler<UserContext, UserRequest>
    {

    }
}
