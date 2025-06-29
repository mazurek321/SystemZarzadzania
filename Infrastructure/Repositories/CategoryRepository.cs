using Microsoft.EntityFrameworkCore;
using Infrastructure.Database;
using Core.Models.Categories;

namespace Infrastructure.Repositories;
internal sealed class CategoryRepository(AppDbContext dbContext) : ICategoryRepository
{
    public async Task AddCategoryAsync(Category category)
    {
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
    }
    public async Task<ICollection<Category>> BrowseCategoriesAsync(int pageNumber, int pageSize)
    {
        return await dbContext.Categories
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
    }
    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        return await dbContext.Categories.FirstOrDefaultAsync(x=>x.Id == id);
    }
    public async Task DeleteCategoryAsync(Category category)
    {
        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync();
    }
}