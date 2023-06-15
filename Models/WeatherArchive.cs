namespace WeatherProject.Models;

public class WeatherArchive
{
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public decimal Temperature { get; set; }
    public decimal Humidity { get; set; }
    public decimal DewPoint { get; set; }
    public int Pressure { get; set; }
    public int? WindSpeed { get; set; }
    public int? CloudCoverPercentage { get; set; }
    public int? CloudBase { get; set; }
    public decimal? Visibility { get; set; }
    public string? WeatherType { get; set; }
    public List<WindDirection> WindDirections { get; set; } = new();
}