using EFDemo.Infra.Entities;
using EFDemo.Infra.Entities.Discriminator;
using EFDemo.Infra.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EFDemo.Infra.Context;

public class MovieDbContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Director> Directors { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<MoviePhoto> MoviePhoto { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Documentary> Documentaries { get; set; }
    public DbSet<TvShow> TvShows { get; set; }

    public MovieDbContext(DbContextOptions options) : base(options)
    {
    }

    public MovieDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ef");

        //IEntityTypeConfiguration kullanan configleri otomatik ekle
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MovieEntityConfiguration).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //if (optionsBuilder.IsConfigured)
        //    return;

        //var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        //var connStr = configuration.GetConnectionString("SqlServer");

        //optionsBuilder.UseSqlServer(connStr, options =>
        //{
        //    options.CommandTimeout(5_000);
        //    options.EnableRetryOnFailure(maxRetryCount: 5);
        //});
    }

}
public class DbContextFactory : IDesignTimeDbContextFactory<MovieDbContext>
{
    public MovieDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var connStr = configuration.GetConnectionString("SqlServer");

        var optionBuilder = new DbContextOptionsBuilder<MovieDbContext>();

        optionBuilder.UseSqlServer(connStr, options =>
        {
            options.MigrationsHistoryTable("__EfMigrationHistory", schema: "ef");
            options.CommandTimeout(5_000);
            //options.EnableRetryOnFailure(maxRetryCount: 5);
        });

        optionBuilder.LogTo(m => Log(m));

        return new MovieDbContext(optionBuilder.Options);
    }

    void Log(string message)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("EFCORE_MESSAGE: {0}", message);
        Console.ForegroundColor = color;
    }
}
