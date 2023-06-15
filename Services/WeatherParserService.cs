using System.Diagnostics;
using System.Globalization;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using WeatherProject.Models;
using WeatherProject.Repositories;

namespace WeatherProject.Services;

public class WeatherParserService : IWeatherParserService
{
    private readonly ILogger<WeatherParserService> _logger;
    private readonly IWeatherArchiveRepository _weatherArchiveRepository;

    private List<WindDirection> _windDirections = new();

    public WeatherParserService(IWeatherArchiveRepository weatherArchiveRepository,
        ILogger<WeatherParserService> logger)
    {
        _weatherArchiveRepository = weatherArchiveRepository;
        _logger = logger;
    }

    public async Task ParseAndSave(IEnumerable<Stream> fileStreams)
    {
        var sw = new Stopwatch();
        sw.Start();

        _windDirections = await _weatherArchiveRepository.GetWindDirections();
        List<WeatherArchive> weatherArchives = new();

        var tasks = fileStreams
            .Select(fileStream => Task.Run(() => GetWeatherArchiveDataFromExcel(fileStream)))
            .ToList();

        await Task.WhenAll(tasks);

        foreach (var task in tasks) weatherArchives.AddRange(task.Result);

        _logger.LogInformation("WeatherParserService.ParseAndSave Finished in {ElapsedMilliseconds}ms",
            sw.ElapsedMilliseconds);

        await _weatherArchiveRepository.AddWeatherArchiveBulk(weatherArchives);
    }

    private async Task<List<WeatherArchive>> GetWeatherArchiveDataFromExcel(Stream fileStream)
    {
        var workbook = new XSSFWorkbook(fileStream);
        var sheetCount = workbook.NumberOfSheets;

        List<WeatherArchive> weatherArchives = new();
        List<Task<List<WeatherArchive>>> parsingTasks = new();

        for (var i = 0; i < sheetCount; i++)
        {
            var sheet = workbook.GetSheetAt(i);

            var sheetParsingTask = Task.Run(() =>
            {
                List<WeatherArchive> sheetWeatherArchive = new();

                for (var t = 4; t < sheet.LastRowNum; t++)
                {
                    var weatherArchive = GetWeatherArchiveDataFromExcel(sheet.GetRow(t));
                    sheetWeatherArchive.Add(weatherArchive);
                }

                return sheetWeatherArchive;
            });

            parsingTasks.Add(sheetParsingTask);
        }

        await Task.WhenAll(parsingTasks);

        foreach (var task in parsingTasks) weatherArchives.AddRange(task.Result);

        return weatherArchives;
    }

    private WeatherArchive GetWeatherArchiveDataFromExcel(IRow row)
    {
        var date = GetValueFromCell<DateOnly>(row.GetCell(0));
        var time = GetValueFromCell<TimeOnly>(row.GetCell(1));
        var temperature = GetValueFromCell<decimal>(row.GetCell(2));
        var humidity = GetValueFromCell<decimal>(row.GetCell(3));
        var dewPoint = GetValueFromCell<decimal>(row.GetCell(4));
        var pressure = GetValueFromCell<int>(row.GetCell(5));
        var windDirectionString = GetValueFromCell<string>(row.GetCell(6));
        var windDirections = GetWindDirectionsByNames(windDirectionString ?? string.Empty);
        var windSpeed = GetValueFromCell<int>(row.GetCell(7));
        var cloudCoverPercentage = GetValueFromCell<int>(row.GetCell(8));
        var cloudBase = GetValueFromCell<int>(row.GetCell(9));
        var visibility = GetValueFromCell<decimal>(row.GetCell(10));
        var weatherType = GetValueFromCell<string>(row.GetCell(11));

        return new WeatherArchive
        {
            Date = date,
            Time = time,
            Temperature = temperature,
            Humidity = humidity,
            DewPoint = dewPoint,
            Pressure = pressure,
            WindSpeed = windSpeed,
            CloudCoverPercentage = cloudCoverPercentage,
            CloudBase = cloudBase,
            Visibility = visibility,
            WeatherType = weatherType,
            WindDirections = windDirections
        };
    }

    private List<WindDirection> GetWindDirectionsByNames(string names)
    {
        List<WindDirection> windDirections = new();
        var namesArray = names.Split(',');

        foreach (var windDirectionName in namesArray)
        {
            if (string.IsNullOrWhiteSpace(windDirectionName)) continue;

            var windDirection = _windDirections.FirstOrDefault(x => x.Name == windDirectionName);

            if (windDirection != null) windDirections.Add(windDirection);
        }

        return windDirections;
    }

    private T? GetValueFromCell<T>(ICell? cell)
    {
        if (cell == null) return default;

        var value = cell.ToString();

        var parsedValue = default(T);

        if (value == null || string.IsNullOrWhiteSpace(value)) return parsedValue;


        if (typeof(T) == typeof(DateOnly))
        {
            var dateTime = DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            parsedValue = (T)(object)DateOnly.FromDateTime(dateTime);
        }

        if (typeof(T) == typeof(TimeOnly))
        {
            var time = TimeOnly.ParseExact(value, "HH:mm", CultureInfo.InvariantCulture);
            parsedValue = (T)(object)time;
        }

        if (typeof(T) == typeof(string)) parsedValue = (T)(object)value;

        if (typeof(T) == typeof(int))
            if (int.TryParse(value, out var intValue))
                parsedValue = (T)(object)intValue;
        
        if (typeof(T) == typeof(decimal))
            if (decimal.TryParse(value, out var decimalValue))
                parsedValue = (T)(object)decimalValue;

        return parsedValue;
    }
}