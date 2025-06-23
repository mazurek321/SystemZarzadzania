namespace Core.Models;

public class User
{
    public User(){}
    public User(Guid id, string name, string lastname, string password, string email, UserRole role, DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        Name = name;
        Lastname = lastname;
        PasswordHash = password;
        Email = email;
        Role = role;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Lastname { get; private set; }
    public string PasswordHash { get; private set; }
    public string Email { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static User CreateUser(string name, string lastname, string password, string email)
    {
        return new User(Guid.NewGuid(), name, lastname, password, email, UserRole.User, DateTime.Now, DateTime.Now);
    }

    public void SetPasswordHash(string hashedPassword)
    {
        PasswordHash = hashedPassword;
    }

    public void UpdateUser(string name, string lastname, string email)
    {
        Name = name;
        Lastname = lastname;
        Email = email;
    }


    public enum UserRole
    {
        User,
        Premium,
        Admin
    }

}