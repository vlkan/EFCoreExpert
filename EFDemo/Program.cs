using EFDemo.Infra.Context;
using EFDemo.Infra.Entities;
using Microsoft.Data.SqlClient;
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

optionBuilder.UseLazyLoadingProxies(builder => builder.IgnoreNonVirtualNavigations(true));

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

void PrintMovieNamesWithPhotoUrlWithLazyLoading()
{
    var movie = dbContext.Movies.FirstOrDefault();

    foreach (var item in movie.Photos)
    {
        Console.WriteLine(item.Url);
    }
}

void AddTestGenre()
{
    var genre = new Genre()
    {
        Id = Guid.NewGuid(),
        Name = "Drama",
        CreatedAt = DateTime.Now,
    };

    dbContext.Genres.Add(genre);
    dbContext.SaveChanges();
}

void AddTestMovie()
{
    var movie = new Movie()
    {
        Id = Guid.NewGuid(),
        Name = "New Movie 1",
        CreatedAt = DateTime.Now,
        ViewCount = 1,
        GenreId = Guid.Parse("727725DF-D7F7-451F-BA29-3DB3CB165BBF"),
        Director = new Director()
        {
            Id = Guid.NewGuid(),
            FirstName = "Zeki",
            LastName = "Demirkubuz",
            CreatedAt = DateTime.Now,
        }
    };

    //dbContext.Movies.Add(movie);
    dbContext.Add(movie);
    dbContext.SaveChanges();

    Console.WriteLine("Movie Added");

    /* 
     EFCORE_MESSAGE: warn: 4.02.2025 09:48:09.267 CoreEventId.SensitiveDataLoggingEnabledWarning[10400] (Microsoft.EntityFrameworkCore.Infrastructure)
      Sensitive data logging is enabled. Log entries and exception messages may include sensitive application data; this mode should only be enabled during development.
EFCORE_MESSAGE: dbug: 4.02.2025 09:48:10.001 RelationalEventId.ConnectionOpened[20001] (Microsoft.EntityFrameworkCore.Database.Connection)
      Opened connection to database 'EFDemo' on server '(localdb)\MSSQLLocalDB'.
EFCORE_MESSAGE: info: 4.02.2025 09:48:10.070 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (35ms) [Parameters=[@p0='db512e27-d0f1-4d55-802b-906928a7e3ab', @p1='2025-02-04T09:48:08.3798115+03:00', @p2=NULL (DbType = DateTime2), @p3='Drama' (Nullable = false) (Size = 100)], CommandType='Text', CommandTimeout='5000']
      SET IMPLICIT_TRANSACTIONS OFF;
      SET NOCOUNT ON;
      INSERT INTO [ef].[Genres] ([Id], [CreatedAt], [ModifiedAt], [Name])
      VALUES (@p0, @p1, @p2, @p3);
EFCORE_MESSAGE: dbug: 4.02.2025 09:48:10.086 RelationalEventId.ConnectionClosed[20003] (Microsoft.EntityFrameworkCore.Database.Connection)
      Closed connection to database 'EFDemo' on server '(localdb)\MSSQLLocalDB' (4ms).
EFCORE_MESSAGE: dbug: 4.02.2025 09:48:10.171 RelationalEventId.ConnectionOpened[20001] (Microsoft.EntityFrameworkCore.Database.Connection)
      Opened connection to database 'EFDemo' on server '(localdb)\MSSQLLocalDB'.
EFCORE_MESSAGE: info: 4.02.2025 09:48:10.185 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (4ms) [Parameters=[@p0='f2390300-d166-4e1a-a026-7eed3edaccb3', @p1='2025-02-04T09:48:10.0909978+03:00', @p2='Zeki' (Nullable = false) (Size = 100), @p3='Demirkubuz' (Nullable = false) (Size = 100), @p4=NULL (DbType = DateTime2), @p5='6f733107-28ff-448d-a87f-4c961255371c', @p6='2025-02-04T09:48:10.0907506+03:00', @p7='f2390300-d166-4e1a-a026-7eed3edaccb3', @p8='727725df-d7f7-451f-ba29-3db3cb165bbf', @p9=NULL (DbType = DateTime2), @p10='New Movie 1' (Nullable = false) (Size = 200), @p11='1'], CommandType='Text', CommandTimeout='5000']
      SET NOCOUNT ON;
      INSERT INTO [ef].[Directors] ([Id], [CreatedAt], [FirstName], [LastName], [ModifiedAt])
      VALUES (@p0, @p1, @p2, @p3, @p4);
      INSERT INTO [ef].[Movies] ([Id], [CreatedAt], [DirectorId], [GenreId], [ModifiedAt], [Name], [ViewCount])
      VALUES (@p5, @p6, @p7, @p8, @p9, @p10, @p11);
EFCORE_MESSAGE: dbug: 4.02.2025 09:48:10.193 RelationalEventId.ConnectionClosed[20003] (Microsoft.EntityFrameworkCore.Database.Connection)
      Closed connection to database 'EFDemo' on server '(localdb)\MSSQLLocalDB' (0ms).
     */
}

void UpdateTestGenre()
{
    #region Before EF7
    //Before EF7
    //var genre = dbContext.Genres.First(g => g.Name == "Drama");
    //genre.Name = "Drama1";
    //genre.ModifiedAt = DateTime.Now;

    ////dbContext.Entry(genre).State = EntityState.Modified;
    //dbContext.SaveChanges();
    /* SQL RAW
     EFCORE_MESSAGE: info: 4.02.2025 14:46:44.146 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (32ms) [Parameters=[], CommandType='Text', CommandTimeout='5000']
      SELECT TOP(1) [g].[Id], [g].[CreatedAt], [g].[ModifiedAt], [g].[Name]
      FROM [ef].[Genres] AS [g]
      WHERE [g].[Name] = N'Drama'
EFCORE_MESSAGE: dbug: 4.02.2025 14:46:44.247 RelationalEventId.ConnectionClosed[20003] (Microsoft.EntityFrameworkCore.Database.Connection)
      Closed connection to database 'EFDemo' on server '(localdb)\MSSQLLocalDB' (6ms).
EFCORE_MESSAGE: dbug: 4.02.2025 14:46:44.354 RelationalEventId.ConnectionOpened[20001] (Microsoft.EntityFrameworkCore.Database.Connection)
      Opened connection to database 'EFDemo' on server '(localdb)\MSSQLLocalDB'.
EFCORE_MESSAGE: info: 4.02.2025 14:46:44.410 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (42ms) [Parameters=[@p2='727725df-d7f7-451f-ba29-3db3cb165bbf', @p0='2025-02-04T14:46:44.2482781+03:00' (Nullable = true), @p1='Drama1' (Nullable = false) (Size = 100)], CommandType='Text', CommandTimeout='5000']
      SET IMPLICIT_TRANSACTIONS OFF;
      SET NOCOUNT ON;
      UPDATE [ef].[Genres] SET [ModifiedAt] = @p0, [Name] = @p1
      OUTPUT 1
      WHERE [Id] = @p2;
     */
    #endregion

    #region After EF7
    //executeUpdate and exewcuteDelete not require savechanges.
    dbContext.Genres.Where(g => g.Name == "Drama1")
                    .ExecuteUpdate(u => u.SetProperty(g => g.ModifiedAt, DateTime.Now).SetProperty(g => g.Name, "Drama3"));
    #region SQL RAW
    /*
      EFCORE_MESSAGE: info: 4.02.2025 15:07:32.475 RelationalEventId.CommandExecuted[20101] (Microsoft.EntityFrameworkCore.Database.Command)
      Executed DbCommand (14ms) [Parameters=[], CommandType='Text', CommandTimeout='5000']
      UPDATE [g]
      SET [g].[Name] = N'Drama3',
          [g].[ModifiedAt] = GETDATE()
      FROM [ef].[Genres] AS [g]
      WHERE [g].[Name] = N'Drama1'
EFCORE_MESSAGE: dbug: 4.02.2025 15:07:32.487 RelationalEventId.ConnectionClosed[20003] (Microsoft.EntityFrameworkCore.Database.Connection)
      Closed connection to database 'EFDemo' on server '(localdb)\MSSQLLocalDB' (4ms).
     */
    #endregion

    #endregion
}

void DeleteTestGenre()
{
    #region Before EF7
    //var genre = dbContext.Genres.First(g => g.Name == "Drama3");

    ////dbContext.Genres.Remove(genre);
    //dbContext.Remove(genre);

    //dbContext.SaveChanges();
    #endregion

    #region After EF7

    dbContext.Genres.Where(g => g.Name == "Drama3").ExecuteDelete();

    #endregion

}

void GetMovieWithCinemaData()
{
    //OwnedTypes - Many
    var movie = dbContext.Movies.First();

    foreach (var item in movie.ReleaseCinemas)
    {
        Console.WriteLine("Name: {0}, AddressLine1: {1}", item.Name, item.AddressLine1);
    }
}

void GetMovieWithCinemaDataFromJson()
{
    var movie = dbContext.Movies.First();

    //Collection Initializer C# 13
    movie.ReleaseCinemas = []; // new List<ReleaseCinema>();

    movie.ReleaseCinemas.Add(new ReleaseCinema
    {
        Name = "Ankamall AVM Cinema",
        AddressLine1 = "Ankara/Yenimahalle"
    });

    movie.ReleaseCinemas.Add(new ReleaseCinema
    {
        Name = "Atakule Cinema",
        AddressLine1 = "Ankara/Çankaya"
    });

    dbContext.SaveChanges();

    #region Json Data
    /*
     [{"AddressLine1":"Ankara/Yenimahalle","AddressLine2":null,"Name":"Ankamall AVM Cinema"},{"AddressLine1":"Ankara/\u00C7ankaya","AddressLine2":null,"Name":"Atakule Cinema"}]
     */
    #endregion

    #region Get and Querying Json Data
    var movies = dbContext.Movies.First(m => m.ReleaseCinemas.Any(rc => rc.Name == "Ankamall AVM Cinema"));

    #region SQL Query
        /*
          SELECT TOP(1) [m].[Id], [m].[CreatedAt], [m].[DirectorId], [m].[GenreId], [m].[ModifiedAt], [m].[Name], [m].[ViewCount], [m].[Release_Amount], [m].[Release_Date], [m].[MovieReleaseCinemas]
          FROM [ef].[Movies] AS [m]
          WHERE EXISTS (
              SELECT 1
              FROM OPENJSON([m].[MovieReleaseCinemas], '$') WITH (
                  [AddressLine1] nvarchar(200) '$.AddressLine1',
                  [Name] nvarchar(200) '$.Name'
              ) AS [m0]
              WHERE [m0].[Name] = N'Ankamall AVM Cinema')

        SQLServer OPENJSON function filtering inside json (nvarchar(max))
         */
        #endregion

    #endregion
}

void TopActorsByMovies(int topCount = 10)
{
    SqlParameter sqlParam = new SqlParameter("TopCount", topCount);
    FormattableString rawSql = $"""
        EXECUTE ef.GetTopActorsByMovies {sqlParam}
        """;

    var actors = dbContext.Database.SqlQuery<ActorViewModel>(rawSql).ToList();
}

void RawSqlExamples()
{
    #region FromSQL

    //int nameLength = 4;
    //int nameLengthMax = 15;
    //var nameLengthSqlParam = new SqlParameter("pLength", 4);

    //// After EF 7 (FormattableString)
    //var movies = dbContext.Movies
    //    //.FromSql($"SELECT * FROM ef.Movies Where LEN(NAME) > {nameLength}")
    //    .FromSql($"SELECT * FROM ef.Movies Where LEN(NAME) > {nameLengthSqlParam}")
    //    .ToList();

    //// Before EF7 (FormattableString)
    //movies = dbContext.Movies
    //   .FromSqlInterpolated($"SELECT * FROM ef.Movies Where LEN(NAME) > {nameLength}")
    //   .ToList();


    //movies = dbContext.Movies
    //    .FromSqlRaw($"SELECT * FROM ef.Movies Where LEN(NAME) > @p0", nameLength)
    //    .Where(m => m.Name.Length < nameLengthMax)
    //    .ToList();

    #endregion

    #region ExecuteSQL

    //string formattableStringV1 = $"""
    //    UPDATE ef.Genres 
    //    SET 
    //        ModifiedDate = GETUTCDATE(), 
    //        Name = 'Drama1' 
    //    WHERE 
    //        Name = 'Drama2'
    //    """;

    //string nameParameterValue = "Drama2";
    //FormattableString formattableStringV2 = $"""
    //    UPDATE ef.Genres 
    //    SET 
    //        ModifiedDate = GETUTCDATE(), 
    //        Name = {nameParameterValue} 
    //    WHERE 
    //        Name = 'Drama1'
    //    """;

    //SqlParameter nameSqlParameter = new("pNameValue", "Drama1");
    //FormattableString formattableStringV3 = $"""
    //    UPDATE ef.Genres 
    //    SET 
    //        ModifiedDate = GETUTCDATE(), 
    //        Name = {nameSqlParameter}
    //    WHERE 
    //        Name = {nameParameterValue}
    //    """;


    //var rows = dbContext.Database.ExecuteSqlRaw(formattableStringV1);

    //// After EF7
    //rows = dbContext.Database.ExecuteSql(formattableStringV2);

    //rows = dbContext.Database.ExecuteSqlInterpolated(formattableStringV3);

    #endregion

    #region SqlQuery

    Guid genreId = Guid.Parse("0B4BB3C3-C491-4203-BC4E-AAD55698B280");
    FormattableString mostViewedMovieByGenreId = $"""
        SELECT 
            AVG(ViewCount) 
        FROM 
            ef.Movies 
        Where 
            GenreId = {genreId}
        """;

    var mostViewedCounts = dbContext.Database
        .SqlQuery<int>(mostViewedMovieByGenreId)
        .ToList();


    FormattableString actorSql = $"""
        SELECT Id, FirstName + ' ' + LastName as FullName, FirstName FROM ef.Actors
        """;

    var models = dbContext.Database.SqlQuery<ActorViewModel>(actorSql).ToList();

    #endregion
}

//await GetActors();

//await GroupByExample();

//await PrintMovieNamesWithGenreNames();

//PrintMovieNamesWithPhotoUrl();

//PrintMovieNamesWithPhotoUrlWithLazyLoading();

//AddTestGenre();

//AddTestMovie();

//UpdateTestGenre();

//DeleteTestGenre();

//GetMovieWithCinemaData();

GetMovieWithCinemaDataFromJson();

Console.ReadLine();

class ActorViewModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
}

