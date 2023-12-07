using Microsoft.AspNetCore.Mvc;
using StorageAPI.Model;
using StorageAPI.Responses;
using StorageAPI.Services;

namespace StorageAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly ILogger<StorageController> _logger;
        private readonly ICarService _carService;

        public StorageController(ILogger<StorageController> logger, ICarService carService)
        {
            _logger = logger;
            _carService = carService;
        }

        [HttpPost(Name = "Car")]
        public async Task<IActionResult> PostNewCarData(StorageRequest request)
        {
            await _carService.Save(request);
            _logger.LogInformation($"Name: {request?.Name}");
            _logger.LogInformation($"Price: {request?.Price}");
            _logger.LogInformation($"ImageUrl: {request?.ImageUrl}");

            return Ok();
        }

        [HttpGet(Name = "LatestChanges")]
        public async Task<LatestChangesResponse> GetLatestChanges(int limit = 10, string? search = "")
        {
            var latestChanges = await _carService.LatestChanges(limit, search);

            return latestChanges;
        }
    }
}