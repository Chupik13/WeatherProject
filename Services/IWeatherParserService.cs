namespace WeatherProject.Services;

public interface IWeatherParserService
{
    Task ParseAndSave(IEnumerable<Stream> fileStream);
}