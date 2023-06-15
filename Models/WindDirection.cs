using System.Text.Json.Serialization;

namespace WeatherProject.Models;

public class WindDirection
{
    public int Id { get; set; }
    public string Name { get; set; }

    [JsonIgnore] public List<WeatherArchive> WeatherArchives { get; set; } = new();
}