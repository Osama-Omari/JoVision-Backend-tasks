using Microsoft.AspNetCore.Mvc;

namespace Task44.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Greeter : Controller
    {
        [HttpGet]
        public IActionResult Greet([FromQuery] string name="anonymous")
        {
            return Ok($"Hello {name}");
        }
    }
}
