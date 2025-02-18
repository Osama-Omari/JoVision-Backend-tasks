using Microsoft.AspNetCore.Mvc;

namespace Task46.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Greeter : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok($"Hello, Welcome to my API");
        }

        [HttpPost]
        public IActionResult Post([FromForm] string? name)
        {
            name = string.IsNullOrWhiteSpace(name) ? "Anonymous" : name;
            return Ok($"Hello {name}, Welcome to my API");
        }
    }
}
