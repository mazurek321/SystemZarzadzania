using Core.Models.Users;
namespace Api.Dto.UserDtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public User.UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}