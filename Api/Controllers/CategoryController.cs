using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Database;
using Core.Models.Categories;
using Core.Models.Users;
using Infrastructure.Context;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _user;
    private readonly ILogger<UserController> _logger;
    public CategoryController(
        AppDbContext dbContext,
        ICategoryRepository categoryRepository,
        ICurrentUserService user,
        IUserRepository userRepository,
        ILogger<UserController> logger
    )
    {
        _dbContext = dbContext;
        _categoryRepository = categoryRepository;
        _user = user;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddCategory([FromBody] string name)
    {
        var category = Category.NewCategory(name);

        await _categoryRepository.AddCategoryAsync(category);

        _logger.LogInformation("[Create] New category added: {categoryId}", category.Id);

        return Ok(category);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCategoryById([FromQuery] int id)
    {
        var category = await _categoryRepository.GetCategoryByIdAsync(id);

        if (category is null)
            return NotFound("Category not found.");

        return Ok(category);
    }

    [HttpGet("browse")]
    [Authorize]
    public async Task<IActionResult> BrowseCategories([FromQuery] int pageNumber = 1, int pageSize = 25)
    {
        var categories = await _categoryRepository.BrowseCategoriesAsync(pageNumber, pageSize);

        return Ok(categories);
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteCategory([FromQuery] int id)
    {
        var category = await _categoryRepository.GetCategoryByIdAsync(id);

        if (category is null)
            return NotFound("Category not found.");

        await _categoryRepository.DeleteCategoryAsync(category);

        var user = await _userRepository.FindByIdAsync(_user.Id.Value);

        _logger.LogInformation("[Delete] User {userId} deleted category: {category}", user.Id, JsonSerializer.Serialize(category));

        return NoContent();
    }
}