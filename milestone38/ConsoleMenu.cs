using System.Globalization;

public class ConsoleMenu
{
    private readonly AuthService authService;
    private readonly EventService eventService;

    public ConsoleMenu(
        AuthService authService,
        EventService eventService)
    {
        this.authService = authService;
        this.eventService = eventService;
    }

    public void Run()
    {
        User? currentUser = null;

        while (true)
        {
            if (currentUser == null)
            {
                Console.WriteLine("\n=== EVENT PLANNER ===");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");

                Console.Write("Choose: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Register();
                        break;

                    case "2":
                        currentUser = Login();
                        break;

                    case "3":
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                continue;
            }

            Console.WriteLine(
                $"\n=== EVENT PLANNER | {currentUser.Username} ===");

            Console.WriteLine("1. List my events");
            Console.WriteLine("2. Add event");
            Console.WriteLine("3. Find event");
            Console.WriteLine("4. Delete event");
            Console.WriteLine("5. Filter events by type");
            Console.WriteLine("6. Logout");
            Console.WriteLine("7. Exit");

            Console.Write("Choose: ");
            string? eventChoice = Console.ReadLine();

            switch (eventChoice)
            {
                case "1":
                    ListEvents(currentUser);
                    break;

                case "2":
                    AddEvent(currentUser);
                    break;

                case "3":
                    FindEvent(currentUser);
                    break;

                case "4":
                    DeleteEvent(currentUser);
                    break;

                case "5":
                    FilterEvents(currentUser);
                    break;

                case "6":
                    currentUser = null;
                    Console.WriteLine("Logged out.");
                    break;

                case "7":
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    // AUTHENTICATION

    private void Register()
    {
        Console.Write("Username: ");
        string username = Console.ReadLine() ?? "";

        Console.Write("Password: ");
        string password = Console.ReadLine() ?? "";

        try
        {
            User user = authService.Register(username, password);

            Console.WriteLine(
                $"Account '{user.Username}' created successfully.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private User? Login()
    {
        Console.Write("Username: ");
        string username = Console.ReadLine() ?? "";

        Console.Write("Password: ");
        string password = Console.ReadLine() ?? "";

        User? user = authService.Login(username, password);

        if (user == null)
        {
            Console.WriteLine("Invalid username or password.");
            return null;
        }

        Console.WriteLine($"Welcome, {user.Username}!");

        return user;
    }

    // LIST

    private void ListEvents(User user)
    {
        List<Event> events = eventService.GetForUser(user.Id);

        if (events.Count == 0)
        {
            Console.WriteLine("No events found.");
            return;
        }

        foreach (Event eventData in events)
        {
            PrintEvent(eventData);
        }
    }

    // ADD

    private void AddEvent(User user)
    {
        string? eventType = null;

        while (eventType == null)
        {
            eventType = ChooseEventType();

            if (eventType == null)
            {
                Console.WriteLine("Invalid event type.");
            }
        }

        string name = ReadRequired("Event name: ");
        string date = ReadDate();
        string time = ReadTime();
        string location = ReadRequired("Location: ");
        int guestCount = ReadGuestCount();

        Console.Write("Important notes: ");
        string notes = Console.ReadLine() ?? "";

        try
        {
            Event newEvent = eventService.AddForUser(
                user.Id,
                eventType,
                name,
                date,
                time,
                location,
                guestCount,
                notes
            );

            Console.WriteLine(
                $"Event created successfully! ID: {newEvent.Id}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    // FIND

    private void FindEvent(User user)
    {
        int? eventId = ReadEventId();

        if (eventId == null)
        {
            return;
        }

        Event? eventData =
            eventService.FindForUser(user.Id, eventId.Value);

        if (eventData == null)
        {
            Console.WriteLine("Event not found.");
            return;
        }

        PrintEvent(eventData);
    }

    // DELETE

    private void DeleteEvent(User user)
    {
        int? eventId = ReadEventId();

        if (eventId == null)
        {
            return;
        }

        Event? eventData =
            eventService.FindForUser(user.Id, eventId.Value);

        if (eventData == null)
        {
            Console.WriteLine("Event not found.");
            return;
        }

        if (eventService.DeleteForUser(user.Id, eventId.Value))
        {
            Console.WriteLine($"Deleted: {eventData.Name}");
        }
    }

    // FILTER

    private void FilterEvents(User user)
    {
        string? eventType = ChooseEventType();

        if (eventType == null)
        {
            Console.WriteLine("Invalid event type.");
            return;
        }

        List<Event> events =
            eventService.FilterForUser(user.Id, eventType);

        if (events.Count == 0)
        {
            Console.WriteLine("No matching events.");
            return;
        }

        foreach (Event eventData in events)
        {
            PrintEvent(eventData);
        }
    }

    // INPUT HELPERS

    private static string? ChooseEventType()
    {
        Console.WriteLine("1. Wedding (Champagne table)");
        Console.WriteLine("2. Other event");

        Console.Write("Type: ");
        string? choice = Console.ReadLine();

        return choice switch
        {
            "1" => "Wedding",
            "2" => "Other event",
            _ => null
        };
    }

    private static string ReadRequired(string label)
    {
        while (true)
        {
            Console.Write(label);
            string value = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            Console.WriteLine("This field cannot be empty.");
        }
    }

    private static string ReadDate()
    {
        while (true)
        {
            Console.Write("Date (yyyy-MM-dd): ");
            string input = Console.ReadLine() ?? "";

            if (!DateTime.TryParseExact(
                    input,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime date))
            {
                Console.WriteLine("Invalid date format.");
                continue;
            }

            if (date < DateTime.Today)
            {
                Console.WriteLine("Date cannot be in the past.");
                continue;
            }

            if (date > DateTime.Today.AddYears(3))
            {
                Console.WriteLine(
                    "Date cannot be more than 3 years in the future.");
                continue;
            }

            return date.ToString(
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture);
        }
    }

    private static string ReadTime()
    {
        while (true)
        {
            Console.Write("Time (HH:mm): ");
            string input = Console.ReadLine() ?? "";

            if (DateTime.TryParseExact(
                    input,
                    "HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime time))
            {
                return time.ToString(
                    "HH:mm",
                    CultureInfo.InvariantCulture);
            }

            Console.WriteLine("Invalid time format.");
        }
    }

    private static int ReadGuestCount()
    {
        while (true)
        {
            Console.Write("Guest count: ");

            if (int.TryParse(Console.ReadLine(), out int count)
                && count > 0)
            {
                return count;
            }

            Console.WriteLine("Enter a positive number.");
        }
    }

    private static int? ReadEventId()
    {
        Console.Write("Enter event ID: ");

        if (int.TryParse(Console.ReadLine(), out int id)
            && id > 0)
        {
            return id;
        }

        Console.WriteLine("Invalid event ID.");
        return null;
    }

    // DISPLAY

    private static void PrintEvent(Event eventData)
    {
        Console.WriteLine("\n--- EVENT ---");
        Console.WriteLine($"ID: {eventData.Id}");
        Console.WriteLine($"Type: {eventData.Type}");
        Console.WriteLine($"Name: {eventData.Name}");
        Console.WriteLine($"Date: {eventData.Date}");
        Console.WriteLine($"Time: {eventData.Time}");
        Console.WriteLine($"Location: {eventData.Location}");
        Console.WriteLine($"Guest count: {eventData.GuestCount}");
        Console.WriteLine($"Notes: {eventData.Notes}");
        Console.WriteLine();
    }
}