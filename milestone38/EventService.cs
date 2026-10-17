public class EventService
{
    private readonly IEventRepository repository;

    public EventService(IEventRepository repository)
    {
        this.repository = repository;
    }

    // LIST
    public List<Event> GetForUser(int userId)
    {
        return repository.GetAll()
            .Where(e => e.UserId == userId)
            .OrderBy(e => e.Date)
            .ToList();
    }

    // FIND
    public Event? FindForUser(int userId, int eventId)
    {
        return repository.GetAll()
            .FirstOrDefault(e =>
                e.Id == eventId &&
                e.UserId == userId);
    }

    // ADD
    public Event AddForUser(
        int userId,
        string type,
        string name,
        string date,
        string time,
        string location,
        int guestCount,
        string notes)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("Invalid user ID.");
        }

        int nextEventId = repository.GetAll()
            .Select(e => e.Id)
            .DefaultIfEmpty(0)
            .Max() + 1;

        Event newEvent = new Event(
            nextEventId,
            type,
            name,
            date,
            time,
            location,
            guestCount,
            notes,
            userId
        );

        repository.Add(newEvent);

        return newEvent;
    }

    // DELETE
    public bool DeleteForUser(int userId, int eventId)
    {
        Event? eventData = FindForUser(userId, eventId);

        if (eventData == null)
        {
            return false;
        }

        return repository.DeleteById(eventData.Id);
    }

    // FILTER
    public List<Event> FilterForUser(int userId, string eventType)
    {
        return repository.GetAll()
            .Where(e =>
                e.UserId == userId &&
                e.Type == eventType)
            .OrderBy(e => e.Date)
            .ToList();
    }
}