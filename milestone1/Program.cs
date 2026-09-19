Console.WriteLine("=== EVENT PLANNER ===");
Console.WriteLine("1. List all events");
Console.WriteLine("2. Add new event");
Console.WriteLine("3. Find event");
Console.WriteLine("4. Delete event");
Console.WriteLine("5. Exit");

Console.Write("Choose an option: ");
string choice = Console.ReadLine();
int nextEventId = 1; //start from 1
bool running = true;

while (running)
{
    switch (choice)
    {
        case "1":
            Console.WriteLine("Showing all events...");
            break;
        case "2":
            Console.WriteLine("Creating a new event...");
            
            int eventId = nextEventId;
            nextEventId++;
            
            Console.WriteLine("\n--- NEW EVENT ---");
            Console.WriteLine("Select event type:");
            Console.WriteLine("1. Wedding / Champagne table");
            Console.WriteLine("2. Other event");

            Console.Write("Type: ");
            string eventType = Console.ReadLine();
            
            break;
        case "3":
            Console.WriteLine("Finding an event...");
            break;
        case "4":
            Console.WriteLine("Deleting an event...");
            break;
        case "5":
            Console.WriteLine("Exiting...");
            running = false;
            break;
        default:
            Console.WriteLine("Invalid option.");
            break;
    }
    Console.WriteLine();
}

