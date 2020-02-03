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

        public override StepHandlerResult HandleStep()
        {
            this.Context.Response = new StepHandlerResult("SearchCarStep", 200, true);
            return this.Continue();
        }
    }
}