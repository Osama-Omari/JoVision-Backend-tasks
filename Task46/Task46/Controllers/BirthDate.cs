using Microsoft.AspNetCore.Mvc;

namespace Task46.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BirthDate : Controller
    {
        public IActionResult Post([FromForm] string? name, [FromForm] int? years, [FromForm] int? months, [FromForm] int? days)
        {
            name = string.IsNullOrWhiteSpace(name) ? "Anonymous" : name;

            if (years == null || months == null || days == null)
            {
                return Ok($"Hello {name}, I can’t calculate your age without knowing your birthdate!");
            }

            try
            {
                DateTime birthDate = new DateTime(years.Value, months.Value, days.Value);
                int age = DateTime.Now.Year - birthDate.Year;
                if (birthDate > DateTime.Now.AddYears(-age))
                {
                    age--;
                }

                return Ok($"Hello {name}, your age is {age}.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Invalid date input:{ex}");
            }
        }
    }
}
