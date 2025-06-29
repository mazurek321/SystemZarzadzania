using Core.Models.Users;
using Core.Models.Categories;
namespace Api.Dto.TaskDtos;

public class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Deadline { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Priority { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime LastUpdate { get; set; }
    public Guid UpdatedBy { get; set; }
    public ICollection<Guid> Users { get; set; }
    public List<int> Categories { get; set; }
}