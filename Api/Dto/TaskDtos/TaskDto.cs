using Core.Models.Users;
using Core.Models.Categories;
namespace Api.Dto.TaskDtos;

public class TaskDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public DateTime Deadline { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public int Priority { get; init; }
    public Guid CreatedBy { get; init; }
    public DateTime LastUpdate { get; init; }
    public Guid UpdatedBy { get; init; }
    public ICollection<Guid> Users { get; init; }
    public List<int> Categories { get; init; }
}