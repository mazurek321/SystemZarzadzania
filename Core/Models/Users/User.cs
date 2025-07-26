using Core.Models.UserTasks;
namespace Core.Models.Users;

public class User
{
    public User() { }
    public User(Guid id, string name, string lastname, string password, string email,
                string phone, bool isActive, DateTime lastActive, UserRole role, DateTime? roleExpiration,
                DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        Name = name;
        Lastname = lastname;
        PasswordHash = password;
        Email = email;
        Phone = phone;
        IsActive = isActive;
        LastActive = lastActive;
        Role = role;
        RoleExpiration = roleExpiration;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Tasks = new List<UserTask>();
    }
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Lastname { get; private set; }
    public string PasswordHash { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime LastActive { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime? RoleExpiration { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public ICollection<UserTask> Tasks { get; private set; }

    public static User CreateUser(string name, string lastname, string password, string email, string phone)
    {
        return new User(Guid.NewGuid(), name, lastname, password, email, phone, false, DateTime.UtcNow, UserRole.User, null, DateTime.UtcNow, DateTime.UtcNow);
    }

    public static User CreateAdmin(string name, string lastname, string password, string email, string phone)
    {
        return new User(Guid.NewGuid(), name, lastname, password, email, phone, false, DateTime.UtcNow, UserRole.Admin, null, DateTime.UtcNow, DateTime.UtcNow);
    }

    public void SetPasswordHash(string hashedPassword)
    {
        PasswordHash = hashedPassword;
    }

    public void UpdateUser(string name, string lastname, string email, string phone)
    {
        Name = name;
        Lastname = lastname;
        Email = email;
        Phone = phone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(UserRole role, int time)
    {
        Role = role;
        RoleExpiration = DateTime.UtcNow.AddDays(time);
        UpdatedAt = DateTime.UtcNow;
    }

    public void CheckRoleExpiration()
    {
        if (RoleExpiration.HasValue && RoleExpiration.Value <= DateTime.UtcNow)
        {
            Role = UserRole.User;
            RoleExpiration = null;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void UpdateActivity()
    {
        IsActive = !IsActive;
        LastActive = DateTime.UtcNow;
    }


    public enum UserRole
    {
        User,
        Premium,
        Admin
    }

}