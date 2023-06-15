using WeatherProject.Models;

namespace WeatherProject.Services;

public interface IWeatherCacheService
{
    Task<List<WeatherArchive>> GetWeatherArchivesAsync(FilterOptions? filterOptions, int pageNumber, int pageSize);
    void InvalidateWeatherArchiveCache();
}