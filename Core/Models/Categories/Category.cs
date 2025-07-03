using Core.Models.UserTasks;
using System.Text.Json.Serialization;

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
    [JsonIgnore]
    public List<UserTask> Tasks { get; private set; }

    public static Category NewCategory(string name)
    {
        return new Category(name);
    }

}