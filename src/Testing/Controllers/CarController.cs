using Microsoft.AspNetCore.Mvc;
using PipelineR.Interface;
using Testing.Pipes;

namespace Testing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        //private readonly IPipeline<CarContext, CarCreate> CreateCarPipeline;
        private readonly ICarPipelineBuilder CarPipeline;

        public CarController(ICarPipelineBuilder carPipeline)
        {
            CarPipeline = carPipeline;
        }

        //public CarController(IPipeline<CarContext, CarCreate> createCarPipeline)
        //{
        //    CreateCarPipeline = createCarPipeline;
        //}

        [HttpGet]
        public IActionResult Testing()
        {
            //var resp = CreateCarPipeline.Execute(new CarCreate());
            var resp = CarPipeline.Create(new CarCreate());
            return new ObjectResult(resp.Result()) { StatusCode = resp.StatusCode };
        }
    }
}