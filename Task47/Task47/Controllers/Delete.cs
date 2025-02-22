using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Task47.Models;
namespace Task47.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Delete : Controller
    {
        private readonly string storage = Path.Combine(Directory.GetCurrentDirectory(), "Images");

        

        [HttpDelete("delete")]
        public IActionResult delete([FromQuery] string FileName , string FileOwner)
        {
            if (string.IsNullOrWhiteSpace(FileName) || string.IsNullOrWhiteSpace(FileOwner))
                return BadRequest("Bad request. File name and  File owner required");

            string imagepath = Path.Combine(storage, FileName);
            string imagemetadatapath = Path.Combine(storage, FileName + ".json");

            if (!System.IO.File.Exists(imagepath))
                return NotFound("image not found");
            if (System.IO.File.Exists(imagemetadatapath))
            {
                string metadataJson = System.IO.File.ReadAllText(imagemetadatapath);
                var metadata = JsonSerializer.Deserialize<ImageMetaData>(metadataJson);
                if (string.IsNullOrEmpty(metadata?.OwnerName) ||
                    !metadata.OwnerName.Trim().ToLower().Equals(FileOwner.Trim().ToLower()))
                {
                    Console.WriteLine($"File Owner: {FileOwner}");
                    Console.WriteLine($"Metadata Owner: {metadata?.OwnerName}");
                    return StatusCode(403, "The owner's name does not match.");
                }
            }
            else
            {
                return StatusCode(403, "The Metadata for the file not found");
            }

            try
            {
                System.IO.File.Delete(imagepath);
                System.IO.File.Delete(imagemetadatapath);

                return Ok("Deleted");
            }catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
