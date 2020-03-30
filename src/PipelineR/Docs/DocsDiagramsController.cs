using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PipelineR.Docs
{
    [Route("docs-diagrams")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DocsDiagramsController : Controller
    {
        [Route(""), HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}