using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Task47.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Task47.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Retrieve : Controller
    {
        private readonly string storage = Path.Combine(Directory.GetCurrentDirectory(), "Images");
        [HttpGet("retrieve")]
        public IActionResult retrieve([FromQuery]string FileName, [FromQuery] string FileOwner)
        {
            if (string.IsNullOrEmpty(FileName) || string.IsNullOrEmpty(FileOwner))
                return BadRequest("Invalid request data");

            string filepath = Path.Combine(storage, FileName);
            string metadatapath = filepath + ".json";

            if (!System.IO.File.Exists(filepath) || !System.IO.File.Exists(metadatapath))
                return BadRequest("File not found");

            var metadataJson = System.IO.File.ReadAllText(metadatapath);
            var metadata = JsonSerializer.Deserialize<ImageMetaData>(metadataJson);

            if(string.IsNullOrEmpty(metadata?.OwnerName) ||
                    !metadata.OwnerName.Trim().ToLower().Equals(FileOwner.Trim().ToLower()))
            {
                return Forbid();
            }

            try
            {
                var fileBytes = System.IO.File.ReadAllBytes(filepath);
                return File(fileBytes, "image/jpeg");

            }catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
