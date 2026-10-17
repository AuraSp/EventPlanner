using System.Text.Json;

public class JsonUserRepository : IUserRepository
{
    private readonly string filePath;
    private readonly List<User> users;

    public JsonUserRepository(string filePath)
    {
        this.filePath = filePath;
        users = Load();
    }

    public List<User> GetAll()
    {
        return users.ToList();
    }

    public User? FindByUsername(string username)
    {
        return users.FirstOrDefault(u =>
            string.Equals(
                u.Username,
                username,
                StringComparison.OrdinalIgnoreCase));
    }

    public void Add(User user)
    {
        users.Add(user);
        Save();
    }

    private List<User> Load()
    {
        if (!File.Exists(filePath))
        {
            return new List<User>();
        }

        string json = File.ReadAllText(filePath);

        return JsonSerializer.Deserialize<List<User>>(json)
               ?? throw new InvalidDataException("Users JSON cannot be null.");
    }

    private void Save()
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(users, options);
        File.WriteAllText(filePath, json);
    }
}