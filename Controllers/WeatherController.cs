using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherProject.Models;
using WeatherProject.Repositories;
using WeatherProject.Services;

namespace WeatherProject.Controllers;

[Route("[controller]")]
[ApiController]
public class WeatherController : ControllerBase
{
    private readonly ILogger<WeatherController> _logger;
    private readonly IWeatherArchiveRepository _weatherArchiveRepository;
    private readonly IWeatherCacheService _weatherCacheService;
    private readonly IWeatherParserService _weatherParserService;

    public WeatherController(
        ILogger<WeatherController> logger,
        IWeatherCacheService weatherCacheService,
        IWeatherParserService weatherParserService, IWeatherArchiveRepository weatherArchiveRepository)
    {
        _logger = logger;
        _weatherCacheService = weatherCacheService;
        _weatherParserService = weatherParserService;
        _weatherArchiveRepository = weatherArchiveRepository;
    }

    [HttpGet("GetWeatherArchive")]
    public async Task<IActionResult> GetWeatherArchive(int pageSize, int? month, int? year, int pageNumber = 1)
    {
        _logger.LogInformation("WeatherController::GetWeatherArchive");

        try
        {
            var filterOptions = new FilterOptions
            {
                Month = month,
                Year = year
            };
            var weatherArchives =
                await _weatherCacheService.GetWeatherArchivesAsync(filterOptions, pageNumber, pageSize);
            return Ok(weatherArchives);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("ClearWeatherArchive")]
    public async Task<IActionResult> ClearWeatherArchive()
    {
        _logger.LogInformation("WeatherController::ClearWeatherArchive");
        try
        {
            await _weatherArchiveRepository.ClearWeatherArchive();
            _weatherCacheService.InvalidateWeatherArchiveCache();
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetTotalWeatherArchive")]
    public async Task<IActionResult> GetTotalWeatherArchive(int? month, int? year)
    {
        _logger.LogInformation("WeatherController::GetTotalWeatherArchive");
        try
        {
            var filterOptions = new FilterOptions
            {
                Month = month,
                Year = year
            };
            var query = _weatherArchiveRepository.GetWeatherArchive();
            query = filterOptions.ApplyTo(query);
            var totalWeatherArchives = await query.CountAsync();
            return Ok(totalWeatherArchives);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("UploadWeatherArchive")]
    public async Task<IActionResult> UploadWeatherArchive()
    {
        _logger.LogInformation("WeatherController::UploadWeatherArchive");
        try
        {
            var files = Request.Form.Files;

            if (files == null || files.Count == 0) throw new Exception("Файлы не прикреплены");
            var streams = files.Select(x => x.OpenReadStream());

            await _weatherParserService.ParseAndSave(streams);
            _weatherCacheService.InvalidateWeatherArchiveCache();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok();
    }
}