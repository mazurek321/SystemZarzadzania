using Core.Models.Users;
using Core.Models.Categories;
namespace Api.Dto.TaskDtos;

public class CreateTaskDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Priority { get; set; }
    public ICollection<User> Users { get; set; }
    public List<Category> Categories { get; set; }
}