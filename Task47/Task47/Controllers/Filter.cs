using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Text.Json;
using Task47.Models;

namespace Task47.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class Filter : Controller
    {
        private readonly string storage = Path.Combine(Directory.GetCurrentDirectory(), "Images");

        public enum FType
        {
            ByModificationDate,
            ByCreationDateDescending,
            ByCreationDateAscending,
            ByOwner
        };


        [HttpPost("filter")]
        public IActionResult filter([FromForm] DateTime? CreationDate, [FromForm] DateTime? ModificationDate, [FromForm] string? Owner, [FromForm] FType? FilterType)
        {

            if (!FilterType.HasValue)
                return BadRequest("Filter type is required.");
            if (FilterType == FType.ByOwner && string.IsNullOrEmpty(Owner))
                return BadRequest("Owner name is required");
            if (FilterType == FType.ByModificationDate && !ModificationDate.HasValue)
                return BadRequest("Modification date is required for this filter type");
            if ((FilterType == FType.ByCreationDateDescending || FilterType == FType.ByCreationDateAscending) && !CreationDate.HasValue)
                return BadRequest("Creation date is required for this filter type");

                try
                {
                    var metadatafiles = Directory.GetFiles(storage, "*.json");
                    List<JsonData> filterdfiles = new List<JsonData>();

                    foreach (var file in metadatafiles)
                    {
                        var metadata = read(file);
                        if (metadata == null)
                            continue;

                        switch (FilterType)
                        {
                            case FType.ByModificationDate:
                                if (metadata.LastModifiedAt < ModificationDate.Value)
                                    filterdfiles.Add(new JsonData { FileName = Path.GetFileNameWithoutExtension(file), OwnerName = metadata.OwnerName });
                                break;
                            case FType.ByCreationDateDescending:
                                if (metadata.CreatedAt > CreationDate.Value)
                                    filterdfiles.Add(new JsonData { FileName = Path.GetFileNameWithoutExtension(file), OwnerName = metadata.OwnerName });
                                break;

                            case FType.ByCreationDateAscending:
                                if (metadata.CreatedAt > CreationDate.Value)
                                    filterdfiles.Add(new JsonData { FileName = Path.GetFileNameWithoutExtension(file), OwnerName = metadata.OwnerName });
                                break;

                            case FType.ByOwner:
                                if (metadata.OwnerName == Owner)
                                    filterdfiles.Add(new JsonData { FileName = Path.GetFileNameWithoutExtension(file), OwnerName = metadata.OwnerName });
                                break;


                        }
                    }
                    if (!filterdfiles.Any())
                        return Ok(new List<JsonData>());


                    

                    IEnumerable<JsonData> sortedfiles = filterdfiles;

                    switch (FilterType)
                    {
                    case FType.ByModificationDate:
                        sortedfiles = sortedfiles.OrderBy(file => read(Path.Combine(storage, $"{file.FileName}.json"))?.LastModifiedAt);
                        break;
                    case FType.ByCreationDateDescending:
                        sortedfiles = sortedfiles.OrderByDescending(file => read(Path.Combine(storage, $"{file.FileName}.json"))?.CreatedAt);
                        break;
                    case FType.ByCreationDateAscending:
                        sortedfiles = sortedfiles.OrderBy(file => read(Path.Combine(storage, $"{file.FileName}.json"))?.CreatedAt);
                        break;
                    case FType.ByOwner:
                        sortedfiles = sortedfiles.OrderBy(file => file.OwnerName);
                        break;
                    }
                return Ok(sortedfiles.ToList());
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error : {ex}");
                }

        }





        private ImageMetaData? read(string filepath)
        {
            try
            {
                string jsoncontent = System.IO.File.ReadAllText(filepath);
                return JsonSerializer.Deserialize<ImageMetaData>(jsoncontent);
            }
            catch
            {
                return null;
            }
        }

    }
}
