using System.Globalization;
using Dapper;

public class DapperUserRepository : IUserRepository
{
    private readonly SqliteDatabase database;

    public DapperUserRepository(SqliteDatabase database)
    {
        this.database = database;
    }

    public List<User> GetAll()
    {
        using var connection = database.OpenConnection();

        string sql = """
            SELECT Id, Username, PasswordHash, CreatedAt
            FROM Users
            ORDER BY Id;
            """;

        return connection.Query<UserRow>(sql)
            .Select(ToUser)
            .ToList();
    }

    public User? FindByUsername(string username)
    {
        using var connection = database.OpenConnection();

        string sql = """
            SELECT Id, Username, PasswordHash, CreatedAt
            FROM Users
            WHERE Username = @Username COLLATE NOCASE
            LIMIT 1;
            """;

        UserRow? row = connection.QueryFirstOrDefault<UserRow>(
            sql,
            new { Username = username }
        );

        return row == null ? null : ToUser(row);
    }

    public void Add(User user)
    {
        using var connection = database.OpenConnection();

        string sql = """
            INSERT INTO Users (Id, Username, PasswordHash, CreatedAt)
            VALUES (@Id, @Username, @PasswordHash, @CreatedAt);
            """;

        connection.Execute(sql, new
        {
            user.Id,
            user.Username,
            user.PasswordHash,
            CreatedAt = user.CreatedAt.ToString(
                "O",
                CultureInfo.InvariantCulture)
        });
    }

    private static User ToUser(UserRow row)
    {
        DateTime createdAt = DateTime.Parse(
            row.CreatedAt,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind
        );

        return new User(
            row.Id,
            row.Username,
            row.PasswordHash,
            createdAt
        );
    }
}

// Represents one row returned from SQLite.
public class UserRow
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string CreatedAt { get; set; } = "";
}