using Microsoft.EntityFrameworkCore;

public class EfEventRepository : IEventRepository
{
    private readonly EventPlannerDbContext context;

    public EfEventRepository(EventPlannerDbContext context)
    {
        this.context = context;
    }

    // GET ALL EVENTS
    public List<Event> GetAll()
    {
        return context.Events
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .ToList();
    }

    // ADD EVENT
    public void Add(Event eventData)
    {
        context.Events.Add(eventData);
        context.SaveChanges();
    }

    // DELETE EVENT
    public bool DeleteById(int id)
    {
        Event? eventData = context.Events
            .FirstOrDefault(e => e.Id == id);

        if (eventData == null)
        {
            return false;
        }

        context.Events.Remove(eventData);
        context.SaveChanges();

        return true;
    }
}