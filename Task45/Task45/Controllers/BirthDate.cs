using Microsoft.AspNetCore.Mvc;

namespace Task45.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BirthDate : Controller
    {
        public IActionResult Get(string? name = "Anonymous", int? years = null , int? months = null, int? days = null )
        {
            if(years == null || months  == null || days == null)
            {
                return Ok($"Hello {name}, I can't calculate your age without knowing your birthdate!");

            }
            try
            {
                // I made my code to accept the bitrthdate like this for example : years = 2004 & months = 2 & days = 25.
                DateTime birthDate = new DateTime(years.Value, months.Value, days.Value);
                int age = DateTime.Now.Year - birthDate.Year;
                if (birthDate > DateTime.Now.AddYears(-age))
                    age--;
                return Ok($"Hello {name}, your age is {age}");

            } catch(Exception ex)
            {
                return BadRequest($"Invalid date input: {ex}");
            }
                
        }
    }
}
