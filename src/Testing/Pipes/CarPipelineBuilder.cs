using PipelineR;
using PipelineR.Base;
using System;

namespace Testing.Pipes
{
    public interface ICarPipelineBuilder
    {
        RequestHandlerResult Create(CarCreate create);
    }

    public class CarPipelineBuilder : ICarPipelineBuilder
    {
        private readonly IServiceProvider ServiceProvider;

        public CarPipelineBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public RequestHandlerResult Create(CarCreate create)
        {
            return Pipeline<CarContext, CarCreate>
                    .Configure(ServiceProvider)
                    .AddNext<ICreateCarStep>()
                    .Execute(create);
        }
    }
}