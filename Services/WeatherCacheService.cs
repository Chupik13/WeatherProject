using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WeatherProject.Models;
using WeatherProject.Repositories;

namespace WeatherProject.Services;

public class WeatherCacheService : IWeatherCacheService
{
    private readonly MemoryCache _cache;
    private readonly ILogger<WeatherCacheService> _logger;
    private readonly IPaginationService _paginationService;
    private readonly IWeatherArchiveRepository _weatherArchiveRepository;

    public WeatherCacheService(
        IWeatherArchiveRepository weatherArchiveRepository,
        IPaginationService paginationService,
        ILogger<WeatherCacheService> logger,
        MemoryCache cache)
    {
        _weatherArchiveRepository = weatherArchiveRepository;
        _paginationService = paginationService;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<WeatherArchive>> GetWeatherArchivesAsync(FilterOptions? filterOptions, int pageNumber,
        int pageSize)
    {
        var cacheKey = $"WeatherArchive_{filterOptions?.GetHashCode()}_{pageNumber}_{pageSize}";
        _logger.LogInformation("GetWeatherArchivesAsync cacheKey: {CacheKey}", cacheKey);

        if (_cache.TryGetValue(cacheKey, out List<WeatherArchive>? weatherArchives))
        {
            _logger.LogInformation("Hit cache for {CacheKey}", cacheKey);
            return weatherArchives ?? Enumerable.Empty<WeatherArchive>().ToList();
        }

        var query = _weatherArchiveRepository.GetWeatherArchive();

        query = filterOptions?.ApplyTo(query) ?? query;
        query = _paginationService.Paginate(query, pageNumber, pageSize);

        var weatherArchive = await query.ToListAsync();

        _cache.Set(cacheKey, weatherArchive, TimeSpan.FromHours(1));

        return weatherArchive;
    }

    public void InvalidateWeatherArchiveCache()
    {
        _cache.Clear();
    }
}