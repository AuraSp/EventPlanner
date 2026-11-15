public interface IUserRepository
{
    List<User> GetAll();
    User? FindByUsername(string username);
    void Add(User user);
}