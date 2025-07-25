using Core.Models.Users;
using Api.Dto.TaskDtos;
namespace Api.Dto.UserDtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastActive { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public IEnumerable<TaskDto> Tasks { get; set; }
}