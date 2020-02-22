using Microsoft.EntityFrameworkCore;
using net.jancerveny.weatherstation.Common.Models;
using System.Text.Json;

namespace net.jancerveny.weatherstation.DataLayer
{
    public partial class WeatherDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public WeatherDbContext()
        {
        }

        public WeatherDbContext(DbContextOptions<WeatherDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Models.DataSource> DataSources { get; set; }
        public virtual DbSet<Models.Measurement> Measurements { get; set; }
        public virtual DbSet<Models.AggregatedMeasurement> AggregatedMeasurements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.DataSource>(entity =>
            {
                entity.ToTable("data_sources", "Weather");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Created)
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.LastRead)
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Color)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, new JsonSerializerOptions { IgnoreNullValues = true }),
                        v => JsonSerializer.Deserialize<ColorRGBA>(v, new JsonSerializerOptions { IgnoreNullValues = true })
                    );
            });

            modelBuilder.Entity<Models.Measurement>(entity =>
            {
                entity.ToTable("measurements", "Weather");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Timestamp)
                    .HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<Models.AggregatedMeasurement>(entity =>
            {
                entity.ToTable("aggregated_measurements", "Weather");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Timestamp)
                    .HasColumnType("timestamp with time zone");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
