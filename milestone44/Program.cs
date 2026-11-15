using System.Text.Json;
using Microsoft.Data.Sqlite;

// STORAGE SELECTION

Console.WriteLine("=== CHOOSE STORAGE ===");
Console.WriteLine("1. Temporary memory (RAM)");
Console.WriteLine("2. JSON file");
Console.WriteLine("3. SQLite database");

Console.Write("Choose an option: ");
string? storageChoice = Console.ReadLine();

try
{
    IEventRepository eventRepository;
    IUserRepository userRepository;

    if (storageChoice == "1")
    {
        // Events are temporary; users are still stored in JSON.
        eventRepository = new InMemoryEventRepository();
        userRepository = new JsonUserRepository("users.json");
    }
    else if (storageChoice == "2")
    {
        // Store both events and users in JSON files.
        eventRepository = new JsonEventRepository("events.json");
        userRepository = new JsonUserRepository("users.json");
    }
    else if (storageChoice == "3")
    {
        // DATABASE INITIALIZATION

        string databasePath = Path.Combine(
            AppContext.BaseDirectory,
            "data",
            "eventplanner.db"
        );

        SqliteDatabase database = new SqliteDatabase(databasePath);

        database.Initialize();

        Console.WriteLine("SQLite database initialized successfully.");

        // SQL REPOSITORIES

        eventRepository = new DapperEventRepository(database);
        userRepository = new DapperUserRepository(database);
    }
    else
    {
        Console.WriteLine("Invalid storage option.");
        return;
    }


    // PASSWORD SERVICE

    IPasswordService passwordService = new PasswordService();

    // APPLICATION SERVICES

    AuthService authService = new AuthService(
        userRepository,
        passwordService
    );

    EventService eventService = new EventService(
        eventRepository
    );

    // CONSOLE PRESENTATION

    ConsoleMenu menu = new ConsoleMenu(
        authService,
        eventService
    );

    menu.Run();
}
catch (Exception ex) when (
    ex is JsonException or
        SqliteException or
        ArgumentException or
        IOException or
        UnauthorizedAccessException or
        InvalidDataException or
        FormatException)
{
    Console.WriteLine($"Application error: {ex.Message}");
}