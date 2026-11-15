using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class EventPlannerDbContextFactory
    : IDesignTimeDbContextFactory<EventPlannerDbContext>
{
    public EventPlannerDbContext CreateDbContext(string[] args)
    {
        return new EventPlannerDbContext(CreateOptions());
    }

    public static DbContextOptions<EventPlannerDbContext> CreateOptions()
    {
        string folder = Path.Combine(AppContext.BaseDirectory, "data");

        Directory.CreateDirectory(folder);

        string databasePath = Path.Combine(
            folder,
            "eventplanner-ef.db"
        );

        return new DbContextOptionsBuilder<EventPlannerDbContext>()
            .UseSqlite($"Data Source={databasePath}")
            .Options;
    }
}