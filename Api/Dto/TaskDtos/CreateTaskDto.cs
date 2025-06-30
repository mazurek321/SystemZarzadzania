using Core.Models.Users;
using Core.Models.Categories;
using System.ComponentModel.DataAnnotations;
public class CreateTaskDto
{
    [Required]
    public string Title { get; init; }
    public string Description { get; init; }
    public DateTime Deadline { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public int Priority { get; init; }
    public List<Guid>? UsersIds { get; init; } = new List<Guid>();
    public List<int>? CategoriesIds { get; init; } = new List<int>();
}