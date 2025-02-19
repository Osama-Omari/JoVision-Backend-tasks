using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
namespace Task47.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Create : Controller
    {

        private readonly string storage = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            
        public Create()
        {
            if (!Directory.Exists(storage))
                Directory.CreateDirectory(storage);
        }


        [HttpPost("upload")]
        public async  Task<IActionResult> UploadImage([FromForm]IFormFile image ,[FromForm]string owner )
        {
            if (image.Length == 0 || image == null || string.IsNullOrWhiteSpace(owner))
                return BadRequest("Image and owner are required");

            if (Path.GetExtension(image.FileName)?.ToLower() != ".jpg")
                return BadRequest("Only jpg format is allowed");

            string imagepath = Path.Combine(storage, image.FileName);
            string imagemetadatapath = Path.Combine(storage, image.FileName + ".json");

            if (System.IO.File.Exists(imagepath))
                return BadRequest("This image is already exist");

            try
            {
                using (var stream = new FileStream(imagepath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                var metadata = new
                {
                    OwnerName = owner,
                    CreatedAt = DateTime.Now,
                    LastModifiedAt = DateTime.Now
                };
                
                await System.IO.File.WriteAllTextAsync(imagemetadatapath, JsonSerializer.Serialize(metadata));

                return Ok("Image uploaded successfully.");

            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
