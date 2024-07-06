using MDS.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MDS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadImageController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;
        public UploadImageController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImages(List<IFormFile> files)
        {
            var folderName = "MDS";
            var urls = await _cloudinaryService.UploadImagesAsync(files, folderName);

            return Ok(new { urls = urls });
        }

    }
}
