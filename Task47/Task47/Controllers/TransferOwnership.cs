using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using Task47.Models;

namespace Task47.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferOwnership : Controller
    {
        private readonly string storage = Path.Combine(Directory.GetCurrentDirectory(), "images");

        [HttpGet("transfer")]
        public IActionResult Transferownership([FromQuery] string? oldOwner, [FromQuery]string?newOwner)
        {
            if (string.IsNullOrEmpty(oldOwner) || string.IsNullOrEmpty(newOwner))
                return BadRequest("Data are required");
            if (oldOwner.Equals(newOwner, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Both owners are the same");
            if (!Directory.Exists(storage))
                return StatusCode(500, "interval server error. storage directory not found");

            try
            {
                var metadatafiles = Directory.GetFiles(storage, "*.json");
                var updatedfiles = new List<Object>();
                
                foreach(var file in metadatafiles)
                {
                    var metadata = read(file);
                    if (metadata == null || !metadata.OwnerName.Equals(oldOwner, StringComparison.OrdinalIgnoreCase))
                        continue;

                    metadata.OwnerName = newOwner;
                    metadata.LastModifiedAt = DateTime.Now;

                    System.IO.File.WriteAllText(file, JsonSerializer.Serialize(metadata));
                    updatedfiles.Add(new { FileName = Path.GetFileNameWithoutExtension(file), Owner = metadata.OwnerName });
                }

                var newOwnerFiles = metadatafiles
                   .Select(file => new
                   {
                       FileName = Path.GetFileNameWithoutExtension(file),
                       Metadata = read(file)
                   })
                   .Where(x => x.Metadata != null && x.Metadata.OwnerName.Equals(newOwner, StringComparison.OrdinalIgnoreCase))
                   .Select(x => new { x.FileName, x.Metadata.OwnerName })
                   .ToList();

                return Ok(newOwnerFiles);


            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        private ImageMetaData? read(string filepath)
        {
            try
            {
                string jsoncontent = System.IO.File.ReadAllText(filepath);
                return JsonSerializer.Deserialize<ImageMetaData>(jsoncontent);
            }catch
            {
                return null;
            }
        }
    }
}
