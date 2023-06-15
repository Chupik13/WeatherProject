namespace WeatherProject.Models;

public class FilterOptions
{
    private const int MinMonth = 1;
    private const int MaxMonth = 12;
    private const int MinYear = 1900;
    private const int MaxYear = 2100;
    private readonly int? _month;
    private readonly int? _year;

    public int? Month
    {
        get => _month;
        init
        {
            if (value.HasValue && value is < MinMonth or > MaxMonth)
                throw new ArgumentException($"Month must be between {MinMonth} and {MaxMonth}.");
            _month = value;
        }
    }

    public int? Year
    {
        get => _year;
        init
        {
            if (value.HasValue && value is < MinYear or > MaxYear)
                throw new ArgumentException($"Year must be between {MinYear} and {MaxYear}.");
            _year = value;
        }
    }

    public IQueryable<WeatherArchive> ApplyTo(IQueryable<WeatherArchive> query)
    {
        if (Month.HasValue) query = query.Where(x => x.Date.Month == Month.Value);

        if (Year.HasValue) query = query.Where(x => x.Date.Year == Year.Value);

        return query;
    }

    //Ради просчета кэша
    public override int GetHashCode()
    {
        var hash = 17;
        hash = hash * 31 + (Month.HasValue ? Month.Value.GetHashCode() : 0);
        hash = hash * 31 + (Year.HasValue ? Year.Value.GetHashCode() : 0);
        return hash;
    }
}