using Dapper;

public class DapperEventRepository : IEventRepository
{
    private readonly SqliteDatabase database;

    public DapperEventRepository(SqliteDatabase database)
    {
        this.database = database;
    }

    // GET ALL EVENTS
    public List<Event> GetAll()
    {
        using var connection = database.OpenConnection();

        string sql = """
            SELECT Id, UserId, Type, Name, Date, Time,
                   Location, GuestCount, Notes
            FROM Events
            ORDER BY Id;
            """;

        List<EventRow> rows = connection.Query<EventRow>(sql).ToList();

        return rows.Select(row => new Event(
            row.Id,
            row.Type,
            row.Name,
            row.Date,
            row.Time,
            row.Location,
            row.GuestCount,
            row.Notes,
            row.UserId
        )).ToList();
    }

    // ADD EVENT
    public void Add(Event eventData)
    {
        using var connection = database.OpenConnection();

        string sql = """
            INSERT INTO Events
            (Id, UserId, Type, Name, Date, Time,
             Location, GuestCount, Notes)
            VALUES
            (@Id, @UserId, @Type, @Name, @Date, @Time,
             @Location, @GuestCount, @Notes);
            """;

        connection.Execute(sql, new
        {
            eventData.Id,
            eventData.UserId,
            eventData.Type,
            eventData.Name,
            eventData.Date,
            eventData.Time,
            eventData.Location,
            eventData.GuestCount,
            eventData.Notes
        });
    }

    // DELETE EVENT
    public bool DeleteById(int id)
    {
        using var connection = database.OpenConnection();

        string sql = """
            DELETE FROM Events
            WHERE Id = @Id;
            """;

        int affectedRows = connection.Execute(sql, new { Id = id });

        return affectedRows > 0;
    }
}

// Represents one row retrieved from the database.
public class EventRow
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; } = "";
    public string Name { get; set; } = "";
    public string Date { get; set; } = "";
    public string Time { get; set; } = "";
    public string Location { get; set; } = "";
    public int GuestCount { get; set; }
    public string Notes { get; set; } = "";
}