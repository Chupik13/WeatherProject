using Microsoft.EntityFrameworkCore;
using WeatherProject.Models;

namespace WeatherProject.Context;

public class WeatherDbContext : DbContext
{
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options)
        : base(options)
    {
        // Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    public DbSet<WeatherArchive> WeatherArchives { get; set; }
    public DbSet<WindDirection> WindDirections { get; set; }
    public DbSet<WeatherArchiveWindDirection> WeatherArchiveWindDirections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //В ручную обозначаю таблицу для связи, чтобы была возможность эффективно добавлять данные через BulkInsert 
        modelBuilder.Entity<WeatherArchive>(entity =>
        {
            entity
                .HasMany(wa => wa.WindDirections)
                .WithMany(wd => wd.WeatherArchives)
                .UsingEntity<WeatherArchiveWindDirection>();
        });

        //Использую двусоставный ключ для обеспечения уникальности погодных данным
        modelBuilder.Entity<WeatherArchive>()
            .HasKey(wa => new { wa.Date, wa.Time });

        modelBuilder.Entity<WeatherArchiveWindDirection>()
            .HasKey(wawd => new { wawd.WindDirectionId, wawd.WeatherArchiveDate, wawd.WeatherArchiveTime });

        modelBuilder.Entity<WindDirection>()
            .HasData(
                new WindDirection { Id = 8, Name = "С" },
                new WindDirection { Id = 1, Name = "СВ" },
                new WindDirection { Id = 2, Name = "В" },
                new WindDirection { Id = 3, Name = "ЮВ" },
                new WindDirection { Id = 4, Name = "Ю" },
                new WindDirection { Id = 5, Name = "ЮЗ" },
                new WindDirection { Id = 6, Name = "З" },
                new WindDirection { Id = 7, Name = "СЗ" },
                new WindDirection { Id = 9, Name = "штиль" }
            );
    }
}