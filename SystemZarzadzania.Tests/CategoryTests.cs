using SystemZarzadzania.Setup;
using Api.Dto.CategoryDtos;
using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Xunit;
using Api.Dto.CategoryDtos;

namespace SystemZarzadzania.Tests;

public class CategoryTests : IClassFixture<TestFixture>, IAsyncLifetime
{
    private readonly TestFixture _fixture;
    public CategoryTests(TestFixture fixture)
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
    public async Task AddNewCategory_ShouldBeOk()
    {
        await _fixture.RegisterAndLoginAsync();

        var newCategory = new AddCategoryDto
        {
            Name = "NewCategoryName"
        };

        var request = await _fixture.Client.PostAsJsonAsync("api/Category", newCategory);

        request.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddExistingCategory_ShouldBeBadRequest()
    {
        await _fixture.RegisterAndLoginAsync();

        var newCategory = new AddCategoryDto
        {
            Name = "NewCategoryName"
        };

        var request = await _fixture.Client.PostAsJsonAsync("api/Category", newCategory);

        var secondRequest = await _fixture.Client.PostAsJsonAsync("api/Category", newCategory);
        secondRequest.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetExistingCategory_ShouldBeOk()
    {
        await _fixture.RegisterAndLoginAsync();

        var newCategory = new AddCategoryDto
        {
            Name = "NewCategoryName"
        };

        var request = await _fixture.Client.PostAsJsonAsync("api/Category", newCategory);
        var response = await request.Content.ReadFromJsonAsync<GetCategoryDto>();

        var getRequest = await _fixture.Client.GetAsync($"api/Category?id={response.Id}");
        getRequest.StatusCode.ShouldBe(HttpStatusCode.OK);

        var category = await getRequest.Content.ReadFromJsonAsync<GetCategoryDto>();
        category.Id.ShouldBe(response.Id);
        category.Name.ShouldBe(newCategory.Name);
    }

    [Fact]
    public async Task DeleteCategory_ShouldBeNoContent()
    {
        await _fixture.RegisterAndLoginAsync();

        var newCategory = new AddCategoryDto
        {
            Name = "NewCategoryName"
        };

        var request = await _fixture.Client.PostAsJsonAsync("api/Category", newCategory);
        var response = await request.Content.ReadFromJsonAsync<GetCategoryDto>();

        var deleteRequest = await _fixture.Client.DeleteAsync($"api/Category?id={response.Id}");
        deleteRequest.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

     [Fact]
    public async Task DeleteCategory_ShouldBeNotFound()
    {
        await _fixture.RegisterAndLoginAsync();

        var id = 2;
        var deleteRequest = await _fixture.Client.DeleteAsync($"api/Category?id={id}");
        deleteRequest.StatusCode.ShouldBe(HttpStatusCode.NotFound);

    }
}