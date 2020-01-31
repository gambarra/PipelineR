using PipelineR;
using PipelineR.Interface;
using System;

namespace Testing.Pipes
{
    public interface ICreateCarStep : IStepHandler<CarContext>
    {
    }

    public class CreateCarStep : StepHandler<CarContext>, ICreateCarStep
    {
        private readonly CarContext Context;

        public CreateCarStep(CarContext context) : base(context)
        {
            this.Context = context;
        }

        public override RequestHandlerResult HandleStep()
        {
            Console.WriteLine("teste");
            this.Context.Response = new RequestHandlerResult("teste", 200, true);
            return this.Continue();
        }
    }
}