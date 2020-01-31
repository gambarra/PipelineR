using PipelineR;
using PipelineR.Interface;

namespace Testing.Pipes
{
    public class EndCarStep : StepHandler<CarContext>, IEndCarStep
    {
        public EndCarStep(CarContext context) : base(context)
        {
        }

        public override RequestHandlerResult HandleStep()
        {
            return this.Finish("finally");
        }
    }

    public interface IEndCarStep : IStepHandler<CarContext>
    {
    }
}