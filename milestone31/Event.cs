using System.Globalization;

public class Event
{
    public int Id { get; private set; }
    public string Type { get; private set; }
    public string Name { get; private set; }
    public string Date { get; private set; }
    public string Time { get; private set; }
    public string Location { get; private set; }
    public int GuestCount { get; private set; }
    public string Notes { get; private set; }
    
    public Event(
        int id,
        string type,
        string name,
        string date,
        string time,
        string location,
        int guestCount,
        string notes)
    {
        
        if (id <= 0)
            throw new ArgumentException("Event ID must be greater than 0.");

        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Event type cannot be empty.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Event name cannot be empty.");

        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Event location cannot be empty.");

        if (guestCount <= 0)
            throw new ArgumentException("Guest count must be greater than 0.");

        
        if (!DateTime.TryParseExact(
                date,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedDate))
        {
            throw new ArgumentException("Event date must use yyyy-MM-dd format.");
        }

        DateTime today = DateTime.Today;
        DateTime maxDate = today.AddYears(3);

        if (parsedDate < today)
        {
            throw new ArgumentException("Event date cannot be in the past.");
        }

        if (parsedDate > maxDate)
        {
            throw new ArgumentException(
                "Event date cannot be more than 3 years in the future.");
        }
        
        Id = id;
        Type = type;
        Name = name;
        Date = date;
        Time = time;
        Location = location;
        GuestCount = guestCount;
        Notes = notes;
    }
}