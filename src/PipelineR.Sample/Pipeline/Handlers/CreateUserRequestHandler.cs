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

        public CreateUserRequestHandler(UserContext context, Expression<Func<UserContext, UserRequest, bool>> condition) : base(context, condition)
        {
        }

        public override RequestHandlerResult HandleRequest(UserRequest request)
        {
            retryCount++;

            if (retryCount < 2)
                return this.Abort("error", 400);

            this.Context.CreateUserRequestHandlerSuccess = true;
          
            return this.Next();
        }
    }

    public interface ICreateUserRequestHandler : IRequestHandler<UserContext, UserRequest>
    {

    }
}
