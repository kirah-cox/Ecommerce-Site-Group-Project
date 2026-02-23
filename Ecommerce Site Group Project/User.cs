using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text.Json;

[Flags]
public enum Permissions
{
    None = 0,
    EditProducts = 1 << 0,
    EditCategories = 1 << 1,
    EditUsers = 1 << 2,
    All = EditProducts | EditCategories | EditUsers,
}

public enum Role
{
    Admin = 1,
    Employee,
    Customer,
}

public interface IUser
{
    Guid Id { get; }
    string Email { get; set; }
    string UserName { get; set; }
    string? Address { get; set; }
    Role Role { get; set; }
    Permissions Permissions { get; set; }

    Task<bool> Register(string password);
    static Task<IUser?> Login(string email, string password) => throw new NotImplementedException();
    Task<bool> Update();
}

public class User : IUser
{
    private static readonly SemaphoreSlim _usersLock = new(1, 1);

    const string dbPath = "users.json";
    const int hashIterations = 100_000;
    const int hashLength = 32;

    public int Version { get; set; }
    public Guid Id { get; private set; }
    public required string Email { get; set; }
    public required string UserName { get; set; }
    public required string HashedPassword { private get; set; }
    public string? Address { get; set; }
    public Role Role { get; set; }
    public Permissions Permissions { get; set; }

    public User()
    {
        Version = 1;
        Id = Guid.NewGuid();
        Role = Role.Customer;
        Permissions = Permissions.None;
    }

    private static string HashPassword(string password)
    {
        const int iterations = 100_000;
        const int hashLength = 32;
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, hashLength);
        // Password and salt need to be stored in one password string
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        // Seperating password and salt from string format "salt:password"
        var parts = storedHash.Split(':');
        if (parts.Length != 2) return false;
        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] hash = Convert.FromBase64String(parts[1]);

        const int iterations = 100_000;
        const int hashLength = 32;
        byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, hashLength);
        return CryptographicOperations.FixedTimeEquals(hash, computedHash);
    }

    private static async Task<List<User>> LoadUsersAsync()
    {
        if (!File.Exists(dbPath)) return [];
        var json = await File.ReadAllTextAsync(dbPath);
        return JsonSerializer.Deserialize<List<User>>(json) ?? [];
    }

    private static async Task SaveUsersAsync(List<User> users)
    {
        // replace the file directly to avoid file corruption if the program crashes while writing
        var tempFilePath = dbPath + ".tmp";
        var json = JsonSerializer.Serialize(users);
        await File.WriteAllTextAsync(tempFilePath, json);
        if (File.Exists(dbPath)) File.Replace(tempFilePath, dbPath, null);
        else File.Move(tempFilePath, dbPath);
    }

    public async Task<bool> Register(string password)
    {
        await _usersLock.WaitAsync();
        try
        {
            var users = await LoadUsersAsync();
            bool emailTaken = users.Any(u => u.Email.Equals(Email, StringComparison.OrdinalIgnoreCase));
            if (emailTaken) return false;

            HashedPassword = HashPassword(password);
            users.Add(this);
            await SaveUsersAsync(users);
            return true;
        }
        finally
        {
            _usersLock.Release();
        }
    }

    public static async Task<User?> Login(string email, string password)
    {
        await _usersLock.WaitAsync();
        try
        {
            var users = await LoadUsersAsync();

            var match = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (match == null) return null;
            if (!VerifyPassword(password, match.HashedPassword)) return null;
            return match;
        }
        finally
        {
            _usersLock.Release();
        }
    }

    public async Task<bool> Update()
    {
        await _usersLock.WaitAsync();
        try
        {
            var users = await LoadUsersAsync();

            int indexOfUser = users.FindIndex(u => u.Id == Id);
            if (indexOfUser == -1) return false;

            if (string.IsNullOrEmpty(HashedPassword))
                HashedPassword = users[indexOfUser].HashedPassword;

            users[indexOfUser] = this;
            await SaveUsersAsync(users);
            return true;
        }
        finally
        {
            _usersLock.Release();
        }
    }
}
