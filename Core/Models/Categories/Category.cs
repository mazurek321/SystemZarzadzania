using Core.Models.UserTasks;
namespace Core.Models.Categories;

public class Category
{
    public Category() { }
    public Category(string name)
    {
        Name = name;
        Tasks = new List<UserTask>();
    }
    public int Id { get; private set; }
    public string Name { get; private set; }
    public List<UserTask> Tasks { get; private set; }

    public static Category NewCategory(string name)
    {
        return new Category(name);
    }

}