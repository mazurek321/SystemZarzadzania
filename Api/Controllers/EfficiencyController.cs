using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Database;
using Api.Dto.TaskDtos;
using Core.Models.UserTasks;
using Core.Models.Users;
using Infrastructure.Context;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ClosedXML.Excel;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EfficiencyController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentUserService _user;
    private readonly IUserRepository _userRepository;
    private readonly IUserTaskRepository _userTaskRepository;
    private readonly ILogger<EfficiencyController> _logger;

    public EfficiencyController(
        AppDbContext dbContext,
        ICurrentUserService user,
        IUserRepository userRepository,
        IUserTaskRepository userTaskRepository,
        ILogger<EfficiencyController> logger)
    {
        _dbContext = dbContext;
        _user = user;
        _userRepository = userRepository;
        _userTaskRepository = userTaskRepository;
        _logger = logger;
    }

    [Authorize]
    [HttpGet("completed-tasks")]
    public async Task<IActionResult> GetCompletedTasksForUser(
        [FromQuery] Guid? userId, DateTime? from, DateTime? to
    )
    {
        if(from > to)
            return BadRequest("Invalid range of date");

        var idToUse = userId.HasValue ? userId.Value : _user.Id.Value;

        var tasks = await _userTaskRepository.GetCompletedTasks(from, to, idToUse);

        var dtos = tasks.Select(t => MapToDto(t)).ToList();

        return Ok(dtos);
    }

    [Authorize]
    [HttpGet("completed-tasks/export")]
    public async Task<IActionResult> ExportCompletedTasksForUser(
        [FromQuery] Guid? userId, DateTime? from, DateTime? to,
        [FromQuery] string format="excel"
    )
    {
        if(from > to)
            return BadRequest("Invalid range of date");

        var idToUse = userId.HasValue ? userId.Value : _user.Id.Value;

        var tasks = await _userTaskRepository.GetCompletedTasks(from, to, idToUse);

        var user = await _userRepository.FindByIdAsync(idToUse);
        if (user is null)
            return NotFound("User not found.");

        if (format.ToLower() == "excel")
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Completed Tasks");

            var title = worksheet.Range(1, 1, 1, 5).Merge();
            title.Value = $"Completed tasks {(from.HasValue && to.HasValue ? $"{from.Value:yyyy-MM-dd} - {to.Value:yyyy-MM-dd}" : "")} [{tasks.Count()}]";
            title.Style.Font.Bold = true;
            title.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            title.Style.Font.FontSize = 16;

            var userInfo = worksheet.Range(2, 1, 2, 5).Merge();
            userInfo.Value = $"{user.Name} {user.Lastname}, {user.Email}";

            worksheet.Cell(4, 1).Value = "Title";
            worksheet.Cell(4, 2).Value = "Description";
            worksheet.Cell(4, 3).Value = "Deadline";
            worksheet.Cell(4, 4).Value = "StartDate";
            worksheet.Cell(4, 5).Value = "EndDate";

            worksheet.Range(4, 1, 4, 5).Style.Font.Bold = true;

            int row = 5;
            foreach (var task in tasks)
            {
                worksheet.Cell(row, 1).Value = task.Title;
                worksheet.Cell(row, 2).Value = task.Description;
                worksheet.Cell(row, 3).Value = task.Deadline;
                worksheet.Cell(row, 4).Value = task.StartDate;
                worksheet.Cell(row, 5).Value = task.EndDate;

                row++;
            }

            worksheet.Columns().AdjustToContents();
            foreach (var col in worksheet.ColumnsUsed())
            {
                col.Width += 5;
            }

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);
            var content = stream.ToArray();
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"CompletedTasks-{user.Email}.xlsx");
        }
        else if (format.ToLower() == "pdf")
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text($"Completed tasks report {(from.HasValue && to.HasValue ? $"{from.Value:yyyy-MM-dd} - {to.Value:yyyy-MM-dd}" : "")} [{tasks.Count()}]")
                        .SemiBold().FontSize(12).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Title");
                                header.Cell().Element(CellStyle).Text("Description");
                                header.Cell().Element(CellStyle).Text("Deadline");
                                header.Cell().Element(CellStyle).Text("Start date");
                                header.Cell().Element(CellStyle).Text("End date");
                                header.Cell().Element(CellStyle).Text("Priority");
                                header.Cell().Element(CellStyle).Text("Users");
                                header.Cell().Element(CellStyle).Text("Categories");
                            });

                            foreach (var task in tasks)
                            {
                                table.Cell().Element(CellStyle).Text(task.Title);
                                table.Cell().Element(CellStyle).Text(task.Description);
                                table.Cell().Element(CellStyle).Text(task.Deadline.ToString("yyyy-MM-dd"));
                                table.Cell().Element(CellStyle).Text(task.StartDate?.ToString("yyyy-MM-dd") ?? "N/A");
                                table.Cell().Element(CellStyle).Text(task.EndDate?.ToString("yyyy-MM-dd") ?? "N/A");
                                table.Cell().Element(CellStyle).Text(task.Priority.ToString());

                                var usersText = task.Users != null && task.Users.Any()
                                    ? string.Join(", ", task.Users.Select(u => u.ToString()))
                                    : "None";
                                table.Cell().Element(CellStyle).Text(usersText);

                                var categoriesText = task.Categories != null && task.Categories.Any()
                                    ? string.Join(", ", task.Categories.Select(c => c.ToString()))
                                    : "None";
                                table.Cell().Element(CellStyle).Text(categoriesText);
                            }
                        });
                });
            });

            IContainer CellStyle(IContainer container)
            {
                return container
                    .Padding(5)
                    .BorderBottom(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .AlignMiddle();
            }

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);

            return File(ms.ToArray(), "application/pdf", $"CompletedTasks-{user.Email}.pdf");
        }


        return BadRequest("Invalid format.");
    }

    



    private TaskDto MapToDto(UserTask task) => new TaskDto
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        Deadline = task.Deadline,
        StartDate = task.StartDate ?? default(DateTime),
        EndDate = task.EndDate ?? default(DateTime),
        Priority = task.Priority,
        CreatedBy = task.CreatedBy,
        LastUpdate = task.LastUpdate,
        UpdatedBy = task.UpdatedBy,
        Users = task.Users?.Select(u => u.Id).ToList() ?? new List<Guid>(),
        Categories = task.Categories?.Select(c => c.Id).ToList() ?? new List<int>()
    };
}
