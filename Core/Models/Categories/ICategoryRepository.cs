
namespace Core.Models.Categories;

public interface ICategoryRepository
{
    Task AddCategoryAsync(Category category);
    Task<ICollection<Category>> BrowseCategoriesAsync(int pageNumber, int pageSize);
    Task<Category> FindCategoryByIdAsync(int id);
    Task DeleteCategoryAsync(Category category);
}