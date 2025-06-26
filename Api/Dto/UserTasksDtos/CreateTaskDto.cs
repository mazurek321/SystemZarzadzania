namespace Api.Dto.UserTasksDtos;

public class CreateTaskDto
{
    public string Title { get; set;}
    public string Description { get; set;}
    public DateTime PlannedBegin { get; set;}
    public DateTime PlannedEnd { get; set;}
    public DateTime ActualBegin { get; set;}
    public DateTime ActualEnd { get; set;}
    public int Priority { get; set;}
    public string Category { get; set;}
}