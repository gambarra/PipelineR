using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineR.Sample.Pipeline.Handlers
{
    public class CreateLoginMark1Handler : RequestHandler<UserContext, UserRequest>, ICreateLoginMark1Handler
    {
        public CreateLoginMark1Handler(UserContext context) : base(context)
        {
        }

        public override RequestHandlerResult HandleRequest(UserRequest request)
        {
            // throw new Exception("teste");

            //return this.Abort("error maroto", 400);

            this.Context.UpdateName(request.Name);
            Console.WriteLine(this.Context.Name);

            return this.Next();
        }
    }

    public interface ICreateLoginMark1Handler : IRequestHandler<UserContext, UserRequest>
    {

    }
}
