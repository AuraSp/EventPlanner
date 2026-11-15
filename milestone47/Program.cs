using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("=== CHOOSE STORAGE ===");
Console.WriteLine("1. Temporary memory (RAM)");
Console.WriteLine("2. JSON file");
Console.WriteLine("3. SQLite (Dapper)");
Console.WriteLine("4. SQLite (Entity Framework Core)");

Console.Write("Choose an option: ");
string? storageChoice = Console.ReadLine();

EventPlannerDbContext? efContext = null;

try
{
    IEventRepository eventRepository;
    IUserRepository userRepository;

    if (storageChoice == "1")
    {
        eventRepository = new InMemoryEventRepository();
        userRepository = new JsonUserRepository("users.json");
    }
    else if (storageChoice == "2")
    {
        eventRepository = new JsonEventRepository("events.json");
        userRepository = new JsonUserRepository("users.json");
    }
    else if (storageChoice == "3")
    {
        string databasePath = Path.Combine(
            AppContext.BaseDirectory,
            "data",
            "eventplanner.db"
        );

        SqliteDatabase database = new SqliteDatabase(databasePath);

        database.Initialize();

        eventRepository = new DapperEventRepository(database);
        userRepository = new DapperUserRepository(database);

        Console.WriteLine("Dapper database initialized.");
    }
    else if (storageChoice == "4")
    {
        efContext = new EventPlannerDbContext(
            EventPlannerDbContextFactory.CreateOptions()
        );

        efContext.Database.Migrate();

        eventRepository = new EfEventRepository(efContext);
        userRepository = new EfUserRepository(efContext);

        Console.WriteLine("EF Core database initialized.");
    }
    else
    {
        Console.WriteLine("Invalid storage option.");
        return;
    }

    IPasswordService passwordService = new PasswordService();

    AuthService authService = new AuthService(
        userRepository,
        passwordService
    );

    EventService eventService = new EventService(
        eventRepository
    );

    ConsoleMenu menu = new ConsoleMenu(
        authService,
        eventService
    );

    menu.Run();
}
catch (Exception ex) when (
    ex is JsonException or
    SqliteException or
    DbUpdateException or
    ArgumentException or
    IOException or
    UnauthorizedAccessException or
    InvalidDataException or
    FormatException or
    InvalidOperationException)
{
    Console.WriteLine($"Application error: {ex.Message}");
}
finally
{
    efContext?.Dispose();
}