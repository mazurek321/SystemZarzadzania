using SystemZarzadzania.Setup;
using Api.Dto.AuthDtos;
using Api.Dto.UserDtos;
using Api.Dto.CategoryDtos;
using Api.Dto.TaskDtos;
using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Xunit;

namespace SystemZarzadzania.Tests;

public class UserTasksTests : IClassFixture<TestFixture>, IAsyncLifetime
{
    private readonly TestFixture _fixture;

    public UserTasksTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();
    }


    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task NewTask_ShouldBeOk()
    {
        await _fixture.RegisterAndLoginAsync();

        var task = new CreateTaskDto
        {
            Title = "Title of task",
            Description = "Description",
            Deadline = DateTime.UtcNow.AddDays(10)
        };

        var request = await _fixture.Client.PostAsJsonAsync("api/UserTasks", task);
        request.StatusCode.ShouldBe(HttpStatusCode.OK);

        var response = await request.Content.ReadFromJsonAsync<TaskDto>();
        response.Title.ShouldBe(task.Title);
    }

    [Fact]
    public async Task NewTaskWithAllProperties_ShouldBeOk()
    {
        var registerRequest = new RegisterDto
        {
            Name = "TesterName2",
            Lastname = "TesterLastname2",
            Password = "TesterPassword2",
            ConfirmPassword = "TesterPassword2",
            Email = "TesterEmail2@Email.com",
            Phone = "123456789"
        };
        var registerResponse = await _fixture.Client.PostAsJsonAsync("api/auth/register", registerRequest);
        registerResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var userId = await registerResponse.Content.ReadFromJsonAsync<UserIdDto>();

        await _fixture.RegisterAndLoginAsync();

        var categoryRequest = new AddCategoryDto
        {
            Name = "NewCategoryName"
        };

        var categoryResponse = await _fixture.Client.PostAsJsonAsync("api/Category", categoryRequest);
        categoryResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var category = await categoryResponse.Content.ReadFromJsonAsync<GetCategoryDto>();

        Console.WriteLine($"USERS: {userId}");
        Console.WriteLine($"CATEGORY: {category.Id}");

        var task = new CreateTaskDto
        {
            Title = "Title of task",
            Description = "Description of task with full parameters",
            Deadline = DateTime.UtcNow.AddDays(10),
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(9),
            Priority = 1,
            UsersIds = new List<Guid> { userId.Id },
            CategoriesIds = new List<int> { category.Id }
        };

        var request = await _fixture.Client.PostAsJsonAsync("/api/UserTasks", task);
        request.StatusCode.ShouldBe(HttpStatusCode.OK);

        var response = await request.Content.ReadFromJsonAsync<TaskDto>();
        response.Title.ShouldBe(task.Title);
    }


}