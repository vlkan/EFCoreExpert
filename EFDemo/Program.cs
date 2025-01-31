using EFDemo.Infra.Context;
using EFDemo.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var connStr = configuration.GetConnectionString("SqlServer");

var optionBuilder = new DbContextOptionsBuilder<MovieDbContext>();

void Log(string message)
{
    var color = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("EFCORE_MESSAGE: {0}", message);
    Console.ForegroundColor = color;
}

optionBuilder.UseSqlServer(connStr, options =>
{
    options.MigrationsHistoryTable("__EfMigrationHistory", schema: "ef");
    options.CommandTimeout(5_000);
    //options.EnableRetryOnFailure(maxRetryCount: 5);
});

optionBuilder.
    LogTo(m => Log(m), (eventId, logLevel) => logLevel >= LogLevel.Information
                                   || eventId == RelationalEventId.ConnectionOpened
                                   || eventId == RelationalEventId.ConnectionClosed)
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging(); // Log yazılırken hassas verilerin görünmesi için.

var dbContext = new MovieDbContext(optionBuilder.Options);

//var actorCount = await dbContext.Actors.Take(5).OrderBy(n => n.FirstName).CountAsync();

//Console.WriteLine("Total actors count: {0}", actorCount);

async Task GetActors()
{

    IQueryable<Actor> query = dbContext.Actors.Where(a => a.FirstName.Contains("A"));

    IQueryable<Actor> queryFunc = dbContext.Actors.Where(a => EF.Functions.Contains(nameof(Actor.FirstName), "%A%"));

    //Deferred Execution

    int actorsCount = query.Count();

    List<Actor> actors = query.ToList();

    List<Guid> ids = query.Select(i => i.Id).ToList(); // select Id from Actors

    var data = query.Select(d => new { d.Id, d.CreatedAt });

    List<ActorViewModel> viewData = query.Select(v => new ActorViewModel()
    {
        Id = v.Id,
        FullName = v.FirstName + " " + v.LastName,
    }).ToList();

    foreach (var item in actors)
    {
        Console.WriteLine("Actor: {0} {1}", item.FirstName, item.LastName);
    }
}

async Task GetMovieWithCount()
{
    int count = dbContext.Movies.Select(m => m.ViewCount).Count();
}

async Task GroupByExample()
{
    var resCount = from m in dbContext.Movies
                   group m by m.GenreId
                   into r
                   select new { Count = r.Count(), GenreId = r.Key };

    foreach (var item in resCount)
    {
        Console.WriteLine(item.Count + " " + item.GenreId);
    }

    var resAvg = dbContext.Movies.GroupBy(m => m.GenreId, m => m.ViewCount, (genreId, viewCounts) => new
    {
        GenreId = genreId,
        AvgViews = viewCounts.Average()
    });

    foreach (var item in resAvg)
    {
        Console.WriteLine(item.GenreId + " " + item.AvgViews);
    }
}

async Task PrintMovieNamesWithGenreNames()
{
    var sqlQuery = dbContext.Movies.Include(m => m.Genre).ToQueryString();
    var allMovies = await dbContext
        .Movies.Include(m => m.Genre)
        .Select(m => new { MovieName = m.Name, GenreName = m.Genre.Name })
        .ToListAsync();

    var filteredQuery = dbContext.Movies
        .Include(g => g.Genre)
        .Include(a => a.Actors.Where(a => a.FirstName.Contains("A")))
        .Where(m => m.Genre.Name == "Comedy")
        .Select(m => new { MovieName = m.Name, GenreName = m.Genre.Name, Actors = m.Actors.Where(b => b.FirstName.Contains("A")) })
        .ToQueryString();

}

void PrintMovieNamesWithPhotoUrl()
{
    var movie = dbContext.Movies.FirstOrDefault();

    dbContext.Entry(movie)
        .Reference(m => m.Photos)
        .Query()
        .Where(i => i.Any(p => p.Url.StartsWith("https")))
        .Load();

    foreach (var item in movie.Photos)
    {
        Console.WriteLine(item.Url);
    }
}

await GetActors();

await GroupByExample();

await PrintMovieNamesWithGenreNames();

void PrintMovieNamesWithPhotoUrl();

Console.ReadLine();

class ActorViewModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
}

