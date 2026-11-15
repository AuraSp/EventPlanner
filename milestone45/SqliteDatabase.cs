using Dapper;
using Microsoft.Data.Sqlite;

public class SqliteDatabase
{
    private readonly string connectionString;

    public SqliteDatabase(string filePath)
    {
        string fullPath = Path.GetFullPath(filePath);

        string? directory = Path.GetDirectoryName(fullPath);

        if (directory != null)
        {
            Directory.CreateDirectory(directory);
        }

        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = fullPath,
            ForeignKeys = true
        };

        connectionString = builder.ToString();
    }

    public SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();

        return connection;
    }

    public void Initialize()
    {
        using var connection = OpenConnection();

        connection.Execute("""
                           CREATE TABLE IF NOT EXISTS Users
                           (
                               Id INTEGER PRIMARY KEY AUTOINCREMENT,
                               Username TEXT NOT NULL COLLATE NOCASE UNIQUE,
                               PasswordHash TEXT NOT NULL,
                               CreatedAt TEXT NOT NULL
                           );

                           CREATE TABLE IF NOT EXISTS Events
                           (
                               Id INTEGER PRIMARY KEY AUTOINCREMENT,
                               UserId INTEGER NOT NULL,
                               Type TEXT NOT NULL,
                               Name TEXT NOT NULL,
                               Date TEXT NOT NULL,
                               Time TEXT NOT NULL,
                               Location TEXT NOT NULL,
                               GuestCount INTEGER NOT NULL CHECK (GuestCount > 0),
                               Notes TEXT NOT NULL DEFAULT '',

                               FOREIGN KEY (UserId) REFERENCES Users(Id)
                           );

                           CREATE INDEX IF NOT EXISTS IX_Events_UserId
                           ON Events(UserId);
                           """);
    }
}