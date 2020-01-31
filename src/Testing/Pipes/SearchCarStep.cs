using PipelineR;
using PipelineR.Interface;
using System;

namespace Testing.Pipes
{
    public interface ISearchCarStep : IStepHandler<CarContext>
    {
    }

    public class SearchCarStep : StepHandler<CarContext>, ISearchCarStep
    {
        private readonly CarContext Context;

        public SearchCarStep(CarContext context) : base(context)
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