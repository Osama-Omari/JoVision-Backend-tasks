using Microsoft.AspNetCore.Mvc;

namespace Task45.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Greeter : Controller
    {
        public IActionResult Greete([FromQuery] string name = "anonymous")
        {
            return Ok($"Hello {name}");
        }
    }
}
