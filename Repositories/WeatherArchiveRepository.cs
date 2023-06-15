using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WeatherProject.Context;
using WeatherProject.Models;

namespace WeatherProject.Repositories;

public class WeatherArchiveRepository : IWeatherArchiveRepository
{
    private readonly WeatherDbContext _dbContext;
    private readonly ILogger<WeatherArchiveRepository> _logger;

    public WeatherArchiveRepository(WeatherDbContext dbContext, ILogger<WeatherArchiveRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public IQueryable<WeatherArchive> GetWeatherArchive()
    {
        return _dbContext
            .WeatherArchives
            .Include(wa => wa.WindDirections)
            .AsNoTracking()
            .OrderBy(x => x.Date)
            .AsQueryable();
    }

    public async Task AddWeatherArchiveBulk(List<WeatherArchive> weatherArchives)
    {
        try
        {
            var sw = Stopwatch.StartNew();

            //В данном случае использую BulkInsert ради эффективности
            //В BulkInsert нет поддержки связности, по этому связываю руками
            var weatherArchiveWindDirections = weatherArchives.SelectMany(wa =>
                wa.WindDirections.Select(wd =>
                    new WeatherArchiveWindDirection
                    {
                        WindDirectionId = wd.Id,
                        WeatherArchiveDate = wa.Date,
                        WeatherArchiveTime = wa.Time
                    }
                )
            );

            await _dbContext.BulkInsertAsync(weatherArchives);
            await _dbContext.BulkInsertAsync(weatherArchiveWindDirections);

            _logger.LogInformation(
                "Added {WeatherArchivesCount} weather archives in {SwElapsedMilliseconds} ms",
                weatherArchives.Count,
                sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding weather archives");
            throw;
        }
    }

    public async Task<List<WindDirection>> GetWindDirections()
    {
        try
        {
            return await _dbContext.WindDirections.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving wind directions");
            throw;
        }
    }

    public async Task ClearWeatherArchive()
    {
        try
        {
            var weatherArchives = await _dbContext.WeatherArchives.ToListAsync();
            var weatherArchivesWindDirections = await _dbContext.WeatherArchiveWindDirections.ToListAsync();
            await _dbContext.BulkDeleteAsync(weatherArchives);
            await _dbContext.BulkDeleteAsync(weatherArchivesWindDirections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while clearing weather archives");
            throw;
        }
    }
}