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
                        .AsNoTracking()
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
    }
    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        return await dbContext.Categories.FirstOrDefaultAsync(x=>x.Id == id);
    }

    public async Task<bool> CheckIfCategoryElareadyExists(string name)
    {
        return await dbContext.Categories.AnyAsync(x=>x.Name == name);
    }

    public async Task<List<Category>> GetByIdsAsync(List<int> list)
    {
        return await dbContext.Categories
                                    .AsNoTracking()
                                    .Where(x => list.Contains(x.Id))
                                    .ToListAsync();
    }
    public async Task DeleteCategoryAsync(Category category)
    {
        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync();
    }
}