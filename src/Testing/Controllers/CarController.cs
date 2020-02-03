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

        [HttpGet("{name}")]
        public IActionResult Testing([FromRoute] string name)
        {
            var req = new CarCreate()
            {
                Nome = name
            };
            var resp = CarPipeline.Create(req);
            return new ObjectResult(resp.Result()) { StatusCode = resp.StatusCode };
        }
    }
}