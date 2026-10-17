using System.Text.Json;

public class JsonEventRepository : IEventRepository
{
    private readonly string filePath;
    private readonly List<Event> events;

    public JsonEventRepository(string filePath)
    {
        this.filePath = filePath;
        events = Load();
    }

    public List<Event> GetAll()
    {
        return events.ToList();
    }

    public void Add(Event eventData)
    {
        events.Add(eventData);
        Save();
    }

    public bool DeleteById(int id)
    {
        Event? eventData = events.FirstOrDefault(e => e.Id == id);

        if (eventData == null)
        {
            return false;
        }

        events.Remove(eventData);
        Save();

        return true;
    }

    private List<Event> Load()
    {
        if (!File.Exists(filePath))
        {
            return new List<Event>();
        }

        string json = File.ReadAllText(filePath);

        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<List<Event>>(json, options)
               ?? new List<Event>();
    }

    private void Save()
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(events, options);

        File.WriteAllText(filePath, json);
    }
}