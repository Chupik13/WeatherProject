using WeatherProject.Models;

namespace WeatherProject.Repositories;

public interface IWeatherArchiveRepository
{
    IQueryable<WeatherArchive> GetWeatherArchive();
    Task AddWeatherArchiveBulk(List<WeatherArchive> weatherArchives);
    Task<List<WindDirection>> GetWindDirections();
    Task ClearWeatherArchive();
}