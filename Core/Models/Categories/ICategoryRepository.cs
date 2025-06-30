
namespace Core.Models.Categories;

public interface ICategoryRepository
{
    Task AddCategoryAsync(Category category);
    Task<ICollection<Category>> BrowseCategoriesAsync(int pageNumber, int pageSize);
    Task<Category> GetCategoryByIdAsync(int id);
    Task<List<Category>> GetByIdsAsync(List<int> list);
    Task DeleteCategoryAsync(Category category);
}