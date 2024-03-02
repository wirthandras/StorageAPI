using FluentValidation;
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
        private IValidator<StorageRequest> _storageRequestValidator;

        public StorageController(ILogger<StorageController> logger, ICarService carService, IValidator<StorageRequest> storageRequestValidator)
        {
            _logger = logger;
            _carService = carService;
            _storageRequestValidator = storageRequestValidator;
        }

        [HttpPost(Name = "Car")]
        public async Task<IActionResult> PostNewCarData(StorageRequest request)
        {
            var validationResult = await _storageRequestValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            await _carService.Save(request);
            _logger.LogInformation($"Name: {request?.Name}");
            _logger.LogInformation($"Price: {request?.Price}");
            _logger.LogInformation($"ImageUrl: {request?.ImageUrl}");

            return Ok();
        }

        [HttpGet("KeyWords")]
        public async Task<KeyWordResponse> GetKeyWords()
        {
            var keyWords = await _carService.GetKeyWords();

            return keyWords;
        }

        [HttpGet("LatestChanges")]
        public async Task<LatestChangesResponse> GetLatestChanges(int limit = 10, string? search = "")
        {
            var latestChanges = await _carService.LatestChanges(limit, search ?? "");

            return latestChanges;
        }

        [HttpGet("RecheckCars")]
        public async Task<IActionResult> PostRecheckCars()
        {
            await _carService.ReCheckCars();

            return Ok();
        }
    }
}