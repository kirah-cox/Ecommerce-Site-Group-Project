[Flags]
public enum Permissions
{
    None = 0,
    ManageProducts   = 1 << 0,
    ManageCategories = 1 << 1,
    ManageUsers      = 1 << 2,
    EditPermissions  = 1 << 3,
    All = ManageProducts | ManageCategories | ManageUsers | EditPermissions,
}

public enum Role
{
  Customer,
  Admin,
}

public interface IUser
{
    Guid Id { get; }
    string Email { get; set; }
    string UserName { get; set; }
    string HashedPassword { set; }
    string? Address { get; set; }
    Role Role { get; set; }
    Permissions Permissions { get; set; }

    Task<bool> Register();
    Task<bool> Login(string email, string password);
    Task<bool> Update();
}

public class User : IUser
{
    public Guid Id { get; private set; }
    public required string Email { get; set; }
    public required string UserName { get; set; }
    public required string HashedPassword { private get; set; }
    public string? Address { get; set; }
    public Role Role { get; set; }
    public Permissions Permissions { get; set; }

    public User()
    {
        Id = Guid.NewGuid();
        Role = Role.Customer;
        Permissions = Permissions.None;
    }
    public async Task<bool> Register()
    {
        return await Task.FromResult(false);
    }
    public async Task<bool> Login(string email, string password)
    {
        return await Task.FromResult(false);
    }
    public async Task<bool> Update()
    {
        return await Task.FromResult(false);
    }
}
