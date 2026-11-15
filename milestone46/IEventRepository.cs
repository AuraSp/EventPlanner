public interface IEventRepository
{
    List<Event> GetAll();
    void Add(Event eventData);
    bool DeleteById(int id);
}