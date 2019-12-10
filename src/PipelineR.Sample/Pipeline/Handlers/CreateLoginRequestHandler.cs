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
            return this.Rollback(new RequestHandlerResult(new ErrorResult("Rollback"), 400));
        }
    }

    public interface ICreateLoginRequestHandler : IRequestHandler<UserContext, UserRequest>
    {

    }
}
