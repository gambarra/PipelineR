using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineR.Sample.Pipeline.Handlers
{
   public  class CreateLoginMark1Handler : RequestHandler<UserContext, UserRequest>, ICreateLoginMark1Handler
    {
        public CreateLoginMark1Handler(UserContext context) : base(context)
        {
        }

        public override RequestHandlerResult HandleRequest(UserRequest request)
        {
            return this.Next();
        }
    }

    public interface ICreateLoginMark1Handler : IRequestHandler<UserContext, UserRequest>
    {

    }
}
