using Microsoft.AspNetCore.Mvc;
using StorageAPI.Model;
using StorageAPI.Requests;
using StorageAPI.Services;

namespace StorageAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ILogger<StorageController> _logger;
        private readonly IImageService _imageService;

        public ImageController(ILogger<StorageController> logger, IImageService imageService)
        {
            _logger = logger;
            _imageService = imageService;
        }

        [HttpPost(Name = "Image")]
        public async Task<IActionResult> PostNewImageData(PostImageRequest request)
        {
            await _imageService.CreateNewImage(request);

            return Ok();
        }

        [HttpPut(Name = "Image")]
        public async Task<IActionResult> PutImageData(PutImageRequest request)
        {
            await _imageService.UpdateInternalImage(request);

            return Ok();
        }
    }
}
