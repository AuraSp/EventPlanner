using Microsoft.EntityFrameworkCore;

public class EfUserRepository : IUserRepository
{
    private readonly EventPlannerDbContext context;

    public EfUserRepository(EventPlannerDbContext context)
    {
        this.context = context;
    }

    // GET ALL USERS
    public List<User> GetAll()
    {
        return context.Users
            .AsNoTracking()
            .ToList();
    }

    // FIND USER BY USERNAME
    public User? FindByUsername(string username)
    {
        return context.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.Username == username);
    }

    // ADD USER
    public void Add(User user)
    {
        context.Users.Add(user);
        context.SaveChanges();
    }
}