using System.Text.Json;

Console.WriteLine("=== CHOOSE STORAGE ===");
Console.WriteLine("1. Temporary memory (RAM)");
Console.WriteLine("2. JSON file");

Console.Write("Choose an option: ");
string? storageChoice = Console.ReadLine();

try
{
    IEventRepository eventRepository;

    if (storageChoice == "1")
    {
        eventRepository = new InMemoryEventRepository();
    }
    else if (storageChoice == "2")
    {
        eventRepository = new JsonEventRepository("events.json");
    }
    else
    {
        Console.WriteLine("Invalid storage option.");
        return;
    }

    IUserRepository userRepository =
        new JsonUserRepository("users.json");

    IPasswordService passwordService =
        new PasswordService();

    AuthService authService = new AuthService(
        userRepository,
        passwordService
    );

    EventService eventService =
        new EventService(eventRepository);

    ConsoleMenu menu = new ConsoleMenu(
        authService,
        eventService
    );
    
    string databasePath = Path.Combine(
        AppContext.BaseDirectory,
        "data",
        "eventplanner.db"
    );

    SqliteDatabase database = new SqliteDatabase(databasePath);

    database.Initialize();

    Console.WriteLine("SQLite database initialized successfully.");
    
    menu.Run();
}
catch (Exception ex) when (
    ex is JsonException or
        IOException or
        ArgumentException or
        UnauthorizedAccessException)
{
    Console.WriteLine($"Application error: {ex.Message}");
}