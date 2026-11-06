using Microsoft.EntityFrameworkCore;

public class EventPlannerDbContext : DbContext
{
    public EventPlannerDbContext(
        DbContextOptions<EventPlannerDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Event> Events => Set<Event>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // USERS

        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .UseCollation("NOCASE")
            .IsRequired();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // EVENTS

        modelBuilder.Entity<Event>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<Event>()
            .Property(e => e.Id)
            .ValueGeneratedNever();

        // ONE USER → MANY EVENTS

        modelBuilder.Entity<Event>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}