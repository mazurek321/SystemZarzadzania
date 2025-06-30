using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Database;
using Api.Dto.TaskDtos;
using Core.Models.Categories;
using Core.Models.UserTasks;
using Core.Models.Users;
using Infrastructure.Context;
using Microsoft.Extensions.Logging;
using System.Text.Json;



namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserTasksController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentUserService _user;
    private readonly IUserRepository _userRepository;
    private readonly IUserTaskRepository _userTaskRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<UserTasksController> _logger;

    public UserTasksController(
        AppDbContext dbContext,
        ICurrentUserService user,
        IUserRepository userRepository,
        IUserTaskRepository userTaskRepository,
        ICategoryRepository categoryRepository,
        ILogger<UserTasksController> logger)
    {
        _dbContext = dbContext;
        _user = user;
        _userRepository = userRepository;
        _userTaskRepository = userTaskRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
    {
        var user = await _userRepository.FindByIdAsync(_user.Id.Value);
        var users = await _userRepository.FindByIdsAsync(dto.UsersIds);
        var categories = await _categoryRepository.GetByIdsAsync(dto.CategoriesIds);

        var task = UserTask.NewTask(
            dto.Title,
            dto.Description,
            dto.Deadline,
            dto.StartDate,
            dto.EndDate,
            dto.Priority,
            user.Id,
            user.Id,
            users,
            categories
        );

        await _userTaskRepository.CreateAsync(task);

        _logger.LogInformation("[Create] User {UserId} created task.", user.Id);
        _logger.LogInformation("[CreateData] Task data: {task}", JsonSerializer.Serialize(task));

        return Ok(task);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetTask([FromQuery] Guid id)
    {
        var task = await _userTaskRepository.GetByIdAsync(id);

        if (task is null)
            return NotFound("Task not found.");

        return Ok(task);
    }

    [HttpGet("browse")]
    [Authorize]
    public async Task<IActionResult> BrowseTasks(
        [FromQuery] int pageNumber = 1, int pageSize = 25,
        [FromQuery] Guid? userId = null,
        [FromQuery] List<int>? categories = null
    )
    {
        var query = await _userTaskRepository.BrowseTasks(pageNumber, pageSize, userId, categories);

        return Ok(query);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateTask([FromQuery] Guid id, [FromBody] UpdateTaskDto dto)
    {
        var task = await _userTaskRepository.GetByIdAsync(id);

        if (task is null)
            return NotFound("Task not found.");

        var user = await _userRepository.FindByIdAsync(_user.Id.Value);

        if (task.CreatedBy != user.Id && !task.Users.Any(x=>x.Id == user.Id) && user.Role != Core.Models.Users.User.UserRole.Admin)
            return BadRequest("You cannot modiy this task.");

        var users = await _userRepository.FindByIdsAsync(dto.UsersIds);
        var categories = await _categoryRepository.GetByIdsAsync(dto.CategoriesIds);

        
        _logger.LogInformation("[Update] User {UserId} updated task {taskId}.", user.Id, task.Id);
        _logger.LogInformation("[UpdateData] Task data before update: {task}", JsonSerializer.Serialize(task));

        task.UpdateTask
        (
            dto.Title,
            dto.Description,
            dto.Deadline,
            dto.StartDate,
            dto.EndDate,
            dto.Priority,
            user.Id,
            users,
            categories
        );

        await _userTaskRepository.UpdateAsync(task);

        _logger.LogInformation("[UpdateData] Task data after update: {task}", JsonSerializer.Serialize(task));

        return Ok(task);
    }

    [HttpPut("users")]
    [Authorize]
    public async Task<IActionResult> UpdateUsersInTask(
        [FromQuery] Guid taskId,
        [FromBody] List<Guid> usersIds
    )
    {
        var task = await _userTaskRepository.GetByIdAsync(taskId);

        if (task is null)
            return NotFound("Task not found.");

        var user = await _userRepository.FindByIdAsync(_user.Id.Value);

        if (task.CreatedBy != user.Id && !task.Users.Any(x=>x.Id == user.Id) && user.Role != Core.Models.Users.User.UserRole.Admin)
            return BadRequest("You cant modiy users within this task."); 

        _logger.LogInformation("[Update] User {UserId} updated task {taskId}.", user.Id, task.Id);
        _logger.LogInformation("[UpdateData] Task data before update: {task}", JsonSerializer.Serialize(task));

        var users = await _userRepository.FindByIdsAsync(usersIds);

        task.UpdateUsers(users);

        await _userTaskRepository.UpdateAsync(task);

        _logger.LogInformation("[UpdateData] Task data after update: {task}", JsonSerializer.Serialize(task));

        return Ok(task);
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteTask([FromQuery] Guid id)
    {
        var task = await _userTaskRepository.GetByIdAsync(id);

        if (task is null)
            return NotFound("Task not found.");
        
        var user = await _userRepository.FindByIdAsync(_user.Id.Value);

        if (task.CreatedBy != user.Id && user.Role != Core.Models.Users.User.UserRole.Admin)
            return BadRequest("You cannot delete this task.");

        await _userTaskRepository.DeleteAsync(task);

        _logger.LogInformation("[Delete] User {userId} deleted task: {task}.", user.Id, JsonSerializer.Serialize(task));

        return NoContent();
    }
}