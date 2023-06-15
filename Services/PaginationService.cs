using WeatherProject.Models;
using WeatherProject.Repositories;

namespace WeatherProject.Services;

public class PaginationService : IPaginationService
{
    private readonly IWeatherArchiveRepository _weatherArchiveRepository;

    public PaginationService(IWeatherArchiveRepository weatherArchiveRepository)
    {
        _weatherArchiveRepository = weatherArchiveRepository;
    }

    public IQueryable<WeatherArchive> Paginate(IQueryable<WeatherArchive> query, int pageNumber, int pageSize)
    {
        return query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }
}