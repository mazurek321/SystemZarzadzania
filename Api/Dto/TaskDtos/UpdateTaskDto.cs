using Core.Models.Users;
using Core.Models.Categories;
namespace Api.Dto.TaskDtos;

public class UpdateTaskDto
{
    public string Title { get; init; }
    public string Description { get; init; }
    public DateTime Deadline { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public int Priority { get; init; }
    public List<Guid> UsersIds { get; init; }
    public List<int> CategoriesIds { get; init; }
}