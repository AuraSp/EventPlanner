public class InMemoryEventRepository : IEventRepository
{
    private readonly List<Event> events = new();

    public List<Event> GetAll()
    {
        return events.ToList();
    }

    public void Add(Event eventData)
    {
        events.Add(eventData);
    }

    public bool DeleteById(int id)
    {
        Event? eventData = events.FirstOrDefault(e => e.Id == id);

        if (eventData == null)
        {
            return false;
        }

        events.Remove(eventData);
        return true;
    }
}