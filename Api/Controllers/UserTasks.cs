using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Database;
using Api.Dto.UserTasksDtos;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserTasksController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    public UserTasksController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
    {
        return Ok();
    }
}