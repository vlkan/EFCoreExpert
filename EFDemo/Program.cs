using EFDemo.Infra.Context;
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

var actorCount = await dbContext.Actors.Take(5).OrderBy(n => n.FirstName).CountAsync();

Console.WriteLine("Total actors count: {0}", actorCount);

Console.ReadLine();