using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Database;
using Api.Dto.TaskDtos;
using Core.Models.UserTasks;
using Core.Models.Users;
using Infrastructure.Context;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserTasksController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentUserService _user;
    private readonly IUserRepository _userRepository;
    private readonly IUserTaskRepository _userTaskRepository;
    public UserTasksController(
        AppDbContext dbContext,
        ICurrentUserService user,
        IUserRepository userRepository,
        IUserTaskRepository userTaskRepository)
    {
        _dbContext = dbContext;
        _user = user;
        _userRepository = userRepository;
        _userTaskRepository = userTaskRepository;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
    {
        var userId = _user.Id.Value;
        var user = await _userRepository.FindByIdAsync(userId);
        var task = UserTask.NewTask
        (
            dto.Title,
            dto.Description,
            dto.StartDate,
            dto.EndDate,
            dto.Priority,
            DateTime.Now,
            user.Id,
            user,
            DateTime.Now,
            user.Id,
            user,
            dto.Users.ToList(),
            dto.Categories
        );

        await _userTaskRepository.CreateAsync(task);

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
        [FromQuery] int pageNumber = 1, int pageSize = 25, Guid? userId = null, List<int>? categories = null
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

        if (task.CreatedBy != user.Id || !task.Users.Any(u => u.Id == user.Id) || user.Role != Core.Models.Users.User.UserRole.Admin)
            return BadRequest("You cannot modiy this task.");

        task.UpdateTask
        (
            dto.Title,
            dto.Description,
            dto.StartDate,
            dto.EndDate,
            dto.Priority,
            DateTime.Now,
            user.Id,
            user,
            dto.Users.ToList(),
            dto.Categories
        );

        await _userTaskRepository.UpdateAsync(task);

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

        if (task.CreatedBy != user.Id || !task.Users.Any(u => u.Id == user.Id) || user.Role != Core.Models.Users.User.UserRole.Admin)
            return BadRequest("You cannot delete this task.");

        await _userTaskRepository.DeleteAsync(task);

        return NoContent();
    }
}