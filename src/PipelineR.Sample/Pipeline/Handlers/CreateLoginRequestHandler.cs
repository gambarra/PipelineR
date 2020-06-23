using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PipelineR.Sample.Pipeline.Handlers
{
    public class CreateLoginRequestHandler:RequestHandler<UserContext, UserRequest>, ICreateLoginRequestHandler
    {
        public CreateLoginRequestHandler(UserContext context) : base(context)
        {
        }

        public CreateLoginRequestHandler(UserContext context, Expression<Func<UserContext, UserRequest, bool>> condition) : base(context, condition)
        {
        }

        public override RequestHandlerResult HandleRequest(UserRequest request)
        {
            throw new Exception("teste");
            return this.Finish("OK");
        }
    }

    public interface ICreateLoginRequestHandler : IRequestHandler<UserContext, UserRequest>
    {

    }
}
