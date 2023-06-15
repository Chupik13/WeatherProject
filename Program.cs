using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using WeatherProject.Context;
using WeatherProject.Repositories;
using WeatherProject.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddMemoryCache();
services.AddSingleton<MemoryCache>();
services.AddDbContext<WeatherDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
services.AddScoped<IWeatherArchiveRepository, WeatherArchiveRepository>();
services.AddScoped<IPaginationService, PaginationService>();
services.AddScoped<IWeatherCacheService, WeatherCacheService>();
services.AddScoped<IWeatherParserService, WeatherParserService>();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();