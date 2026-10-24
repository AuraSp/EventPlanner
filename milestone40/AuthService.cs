public class AuthService
{
    private readonly IUserRepository userRepository;
    private readonly IPasswordService passwordService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordService passwordService)
    {
        this.userRepository = userRepository;
        this.passwordService = passwordService;
    }

    public User Register(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be empty.");
        }

        username = username.Trim();

        if (username.Length < 3 || username.Length > 30)
        {
            throw new ArgumentException(
                "Username must contain between 3 and 30 characters.");
        }

        if (string.IsNullOrEmpty(password) || password.Length < 8)
        {
            throw new ArgumentException(
                "Password must contain at least 8 characters.");
        }

        if (userRepository.FindByUsername(username) != null)
        {
            throw new InvalidOperationException(
                "This username is already registered.");
        }

        int nextUserId = 1;

        foreach (User existingUser in userRepository.GetAll())
        {
            if (existingUser.Id >= nextUserId)
            {
                nextUserId = existingUser.Id + 1;
            }
        }

        string passwordHash = passwordService.HashPassword(password);

        User newUser = new User(
            nextUserId,
            username,
            passwordHash,
            DateTime.UtcNow
        );

        userRepository.Add(newUser);

        return newUser;
    }

    public User? Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrEmpty(password))
        {
            return null;
        }

        User? user = userRepository.FindByUsername(username.Trim());

        if (user == null)
        {
            return null;
        }

        bool validPassword = passwordService.VerifyPassword(
            password,
            user.PasswordHash
        );

        if (!validPassword)
        {
            return null;
        }

        return user;
    }
}