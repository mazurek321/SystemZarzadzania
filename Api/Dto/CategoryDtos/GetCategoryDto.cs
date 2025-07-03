using Core.Models.UserTasks;
namespace Api.Dto.CategoryDtos;

public class GetCategoryDto
{
    public int Id { get; set; }
    public string Name { get; init; }
    public List<UserTask> Tasks { get; init; }
}