using PipelineR;
using PipelineR.Base;
using System;

namespace Testing.Pipes
{
    public interface ICarPipelineBuilder
    {
        StepHandlerResult Create(CarCreate create);
    }

    public class CarPipelineBuilder : ICarPipelineBuilder
    {
        private readonly IServiceProvider ServiceProvider;

        public CarPipelineBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public StepHandlerResult Create(CarCreate create)
        {
            return Pipeline<CarContext, CarCreate>
                    .Configure(ServiceProvider)
                    .AddStep<ISearchCarStep>()
                        .When(p => {
                            var req = (CarCreate)p.Request;
                            return req.Nome != "yuri";
                            })
                    .AddStep<ICreateCarStep>()
                        .When<ISearchCondition>()
                    //.AddFinally<IEndCarStep>()
                    .Execute(create);
        }
    }
}