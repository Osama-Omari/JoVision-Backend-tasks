using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Task47.Models;

namespace Task47.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Update : Controller
    {
        
        private readonly string storage = Path.Combine(Directory.GetCurrentDirectory(), "Images");

        [HttpPost("update")]
        public async Task<IActionResult> UpdateImage([FromForm] IFormFile Image, [FromForm] string Owner)
        {
            if (Image == null ||Image.Length ==0 || string.IsNullOrWhiteSpace(Owner))
                return BadRequest("Image and owner are required.");

            if (Path.GetExtension(Image.FileName)?.ToLower() != ".jpg")
                return BadRequest("Only jpg format is allowed.");

            string imagepath = Path.Combine(storage, Image.FileName);
            string imagemetadatapath = imagepath + ".json";

            if (!System.IO.File.Exists(imagepath) || !System.IO.File.Exists(imagemetadatapath))
                return BadRequest("File not found");

            var metadataJson = System.IO.File.ReadAllText(imagemetadatapath);
            var metadata = JsonSerializer.Deserialize<ImageMetaData>(metadataJson);

            if (string.IsNullOrEmpty(metadata?.OwnerName) ||
                    !metadata.OwnerName.Trim().ToLower().Equals(Owner.Trim().ToLower()))
            {
                return StatusCode(403, "Forbidden");
            }

            try
            {
                using (var stream = new FileStream(imagepath, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }

                metadata.LastModifiedAt = DateTime.Now;
                System.IO.File.WriteAllText(imagemetadatapath, JsonSerializer.Serialize(metadata));

                return Ok("File updated successfully.");
            }catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
