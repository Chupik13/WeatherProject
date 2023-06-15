using WeatherProject.Models;

namespace WeatherProject.Services;

public interface IPaginationService
{
    IQueryable<WeatherArchive> Paginate(IQueryable<WeatherArchive> query, int pageNumber, int pageSize);
}