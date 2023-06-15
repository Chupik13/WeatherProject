namespace WeatherProject.Models;

public class WeatherArchiveWindDirection
{
    public int WindDirectionId { get; set; }
    public DateOnly WeatherArchiveDate { get; set; }
    public TimeOnly WeatherArchiveTime { get; set; }
}