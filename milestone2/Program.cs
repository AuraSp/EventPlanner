

int nextEventId = 1; //start from 1
bool running = true;

while (running)
{
    Console.WriteLine("=== EVENT PLANNER ===");
    Console.WriteLine("1. List all events");
    Console.WriteLine("2. Add new event");
    Console.WriteLine("3. Find event");
    Console.WriteLine("4. Delete event");
    Console.WriteLine("5. Exit");

    Console.Write("Choose an option: ");
    string choice = Console.ReadLine();

    
    switch (choice)
    {
        case "1":
            Console.WriteLine("Showing all events...");
            break;
        case "2":
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

            Console.Write("Date: ");
            string eventDate = Console.ReadLine();

            Console.Write("Time: ");
            string eventTime = Console.ReadLine();

            Console.Write("Location: ");
            string eventLocation = Console.ReadLine();

            
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

}

