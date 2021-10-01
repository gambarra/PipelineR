using PipelineR.Test.Request;

namespace PipelineR.Test.RequestHandlers
{
    public class CreateUserApiFourHandler : RequestHandler<ContextSample, SampleRequest>, ICreateUserApiFourHandler
    {
        public CreateUserApiFourHandler(ContextSample context) : base(context)
        {
        }

        public override RequestHandlerResult HandleRequest(SampleRequest request)
        {
            this.Context.CreateUserApiFourHandlerWasExecuted = true;
            return this.Finish("Success", statusCode: 201);
        }
    }

    public interface ICreateUserApiFourHandler : IRequestHandler<ContextSample, SampleRequest>
    { }
}
