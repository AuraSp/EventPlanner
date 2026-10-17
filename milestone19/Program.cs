using System.Globalization;
using System.Text.Json;

string dataFilePath = "events.json";



List<Event> events = LoadEvents(dataFilePath);

int nextEventId = 1; //start from 1
bool running = true;

foreach (Event eventData in events)
{
    if (eventData.Id >= nextEventId)
    {
        nextEventId = eventData.Id + 1;
    }
}

while (running)
{
    Console.WriteLine("=== EVENT PLANNER ===");
    Console.WriteLine("1. List all events");
    Console.WriteLine("2. Add new event");
    Console.WriteLine("3. Find event");
    Console.WriteLine("4. Delete event");
    Console.WriteLine("5. Filter events by type");
    Console.WriteLine("6. Exit");

    Console.Write("Choose an option: ");
    string choice = Console.ReadLine();

    
    switch (choice)
    {
        case "1":

            ListEvents(events);
            break;
        
        case "2":

            AddEvent(events, ref nextEventId);
            break;
        
        case "3":

            FindEvent(events);
            break;
        
        case "4":
            
            DeleteEvent(events);
            break;
        
        case "5":
            FilterEventsByType(events);
            break;

        case "6":
            Console.WriteLine("Exiting...");
            running = false;
            break;
        
        default:
            Console.WriteLine("Invalid option.");
            break;
    }

}

void ListEvents(List<Event> events)
{
    if (events.Count == 0)
    {
        Console.WriteLine("No events found.");
    }
    else
    {
        foreach (Event eventData in events.OrderBy(e => e.Date))
        {
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
}

// nextEventId++; we want that updated value to survive outside the method
void AddEvent(List<Event> events, ref int nextEventId)
{
 Console.WriteLine("Creating a new event...");
            
            int eventId = nextEventId;
            nextEventId++; //before entering all data, unique id already being assigned
            
            Console.WriteLine("\n--- NEW EVENT ---");
            string eventType = "";

            while (eventType == "") //keep asking until we successfully assign a real event type.
            {
                Console.WriteLine("Select event type:");
                Console.WriteLine("1. Wedding (Champagne table)");
                Console.WriteLine("2. Other event");

                Console.Write("Type: ");
                string eventTypeChoice = Console.ReadLine();

                if (eventTypeChoice == "1")
                {
                    eventType = "Wedding";
                }
                else if (eventTypeChoice == "2")
                {
                    eventType = "Other event";
                }
                else
                {
                    Console.WriteLine("Invalid type. Please choose 1 or 2.");
                }
            }
                        
            //shared fields for both events
            Console.Write("Event name: ");
            string eventName = Console.ReadLine();
            
            string eventDate = "";
            bool validDate = false;

            while (!validDate)
            {
                Console.Write("Date (yyyy-MM-dd): ");
                string dateInput = Console.ReadLine();

                if (DateTime.TryParseExact(
                        dateInput,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime parsedDate))
                {
                    DateTime today = DateTime.Today;
                    DateTime maxDate = today.AddYears(3);

                    if (parsedDate < today)
                    {
                        Console.WriteLine("Event date cannot be in the past.");
                    }
                    else if (parsedDate > maxDate)
                    {
                        Console.WriteLine("Event date cannot be more than 3 years in the future.");
                    }
                    else
                    {
                        eventDate = parsedDate.ToString("yyyy-MM-dd");
                        validDate = true;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid date. Use yyyy-MM-dd.");
                }
            }

            string eventTime = "";
            bool validTime = false;

            while (!validTime)
            {
                Console.Write("Time (HH:mm): ");
                string timeInput = Console.ReadLine();

                if (DateTime.TryParseExact(
                        timeInput,
                        "HH:mm",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime parsedTime))
                {
                    eventTime = parsedTime.ToString("HH:mm");
                    validTime = true;
                }
                else
                {
                    Console.WriteLine("Invalid time. Use HH:mm.");
                }
            }


            string eventLocation = "";

            while (string.IsNullOrWhiteSpace(eventLocation))
            {
                Console.Write("Location: ");
                eventLocation = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(eventLocation))
                {
                    Console.WriteLine("Location cannot be empty.");
                }
            }

            
            /* in here we can use ASCII/UNICODE system to let machine check if symbol can be a number.
             Under the hood, numeric characters such as '0' to '9' have character values.
               For example in ASCII:
               '0' = 48
               '1' = 49
               ...
               '9' = 57
               
                To parse into actual number TryParse uses math trick - [text - 48] = textParsedIntoNumber.
               For a single digit, we manually convert it by subtracting '0' using ASCII:
               '7' - '0' → 55 - 48 = 7.
            
            But user can enter value that can contain several digits, for example "125", so converting it
            manually would require checking every character and building the final number.
            If word would be 'one' -> letter 'o' has 111 code, so 48 != 111 != 57. So for each letter in entered value
            we check using TryParse() and boolean validation.

            */
            int guestCount = 0; // default number for real people
            bool validGuestCount = false;

            while (!validGuestCount)
            {
                Console.Write("Guest count: ");
                string guestInput = Console.ReadLine();

                if (int.TryParse(guestInput, out guestCount) && guestCount > 0)
                    // && guestCount > 0 added, because TryParse considers that -5, -100, 0 are valid integers
                {
                    validGuestCount = true;
                    Console.WriteLine(guestCount);
                }
                else
                {
                    Console.WriteLine("Invalid input. Please choose a number");
                }
            }
            
            
            Console.Write("Important notes: ");
            string notes = Console.ReadLine();
                
            Console.WriteLine("\n--- EVENT CREATED ---");
            Console.WriteLine($"ID: {eventId}");
            Console.WriteLine($"Type: {eventType}");
            Console.WriteLine($"Name: {eventName}");
            Console.WriteLine($"Date: {eventDate}");
            Console.WriteLine($"Time: {eventTime}");
            Console.WriteLine($"Location: {eventLocation}");
            Console.WriteLine($"Guest count: {guestCount}");
            Console.WriteLine($"Notes: {notes}");
            
            Event newEvent = new Event
            (
                eventId,
                eventType,
                eventName,
                eventDate,
                eventTime,
                eventLocation,
                guestCount,
                notes
            );

            events.Add(newEvent);
            
            SaveEvents(dataFilePath, events);
}

void FindEvent(List<Event> events)
{
    Console.Write("Enter event ID: ");

    if (!int.TryParse(Console.ReadLine(), out int searchId))
    {
        Console.WriteLine("Invalid ID.");
        return;
    }

    Event? eventData =
        events.FirstOrDefault(e => e.Id == searchId);

    if (eventData != null)
    {
        Console.WriteLine("\n--- EVENT FOUND ---");
        Console.WriteLine($"ID: {eventData.Id}");
        Console.WriteLine($"Type: {eventData.Type}");
        Console.WriteLine($"Name: {eventData.Name}");
        Console.WriteLine($"Date: {eventData.Date}");
        Console.WriteLine($"Time: {eventData.Time}");
        Console.WriteLine($"Location: {eventData.Location}");
        Console.WriteLine($"Guest count: {eventData.GuestCount}");
        Console.WriteLine($"Notes: {eventData.Notes}");
    }
    else
    {
        Console.WriteLine("Event not found.");
    }
}

void DeleteEvent(List<Event> events)
{
    Console.Write("Enter event ID to delete: ");
    
    if (!int.TryParse(Console.ReadLine(), out int deleteId))
    {
        Console.WriteLine("Invalid ID.");
        return;
    }

    Event? eventToDelete =
        events.FirstOrDefault(e => e.Id == deleteId);

    if (eventToDelete != null)
    {
        Console.WriteLine($"Deleting: {eventToDelete.Name}");

        events.Remove(eventToDelete);

        // Save again so events.json matches the updated events in RAM memory.
        SaveEvents(dataFilePath, events);

        Console.WriteLine("Event deleted.");
    }
    else
    {
        Console.WriteLine("Event not found.");
    }
}

void SaveEvents( string filePath, List<Event> events)
{
    try
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(events, options);

        File.WriteAllText(filePath, json);
    }
    catch (IOException)
    {
        Console.WriteLine("The events file could not be saved.");
    }
}

List<Event> LoadEvents(string filePath)
{
    if (!File.Exists(filePath))
    {
        return new List<Event>();
    }

    try
    {
        string json = File.ReadAllText(filePath);

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        List<Event>? loadedEvents =
            JsonSerializer.Deserialize<List<Event>>(json, options);

        if (loadedEvents != null)
        {
            return loadedEvents;
        }
    }
    catch (JsonException)
    {
        Console.WriteLine("The events file contains invalid JSON.");
    }
    catch (IOException)
    {
        Console.WriteLine("The events file could not be read.");
    }

    return new List<Event>();
}

void FilterEventsByType(List<Event> events)
{
    Console.WriteLine("Choose event type:");
    Console.WriteLine("1. Wedding");
    Console.WriteLine("2. Other event");

    Console.Write("Type: ");
    string choice = Console.ReadLine();

    string eventType = "";

    if (choice == "1")
    {
        eventType = "Wedding";
    }
    else if (choice == "2")
    {
        eventType = "Other event";
    }
    else
    {
        Console.WriteLine("Invalid type.");
        return;
    }

    List<Event> filteredEvents =
        events.Where(e => e.Type == eventType).ToList();

    if (filteredEvents.Count == 0)
    {
        Console.WriteLine("No events found.");
        return;
    }

    foreach (Event eventData in filteredEvents)
    {
        Console.WriteLine($"ID: {eventData.Id}");
        Console.WriteLine($"Type: {eventData.Type}");
        Console.WriteLine($"Name: {eventData.Name}");
        Console.WriteLine($"Date: {eventData.Date}");
        Console.WriteLine($"Location: {eventData.Location}");
        Console.WriteLine();
    }
}