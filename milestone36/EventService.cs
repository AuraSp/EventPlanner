public class EventService
{
    private readonly IEventRepository repository;

    public EventService(IEventRepository repository)
    {
        this.repository = repository;
    }

    public List<Event> GetForUser(int userId)
    {
        return repository.GetAll()
            .Where(e => e.UserId == userId)
            .OrderBy(e => e.Date)
            .ToList();
    }
}