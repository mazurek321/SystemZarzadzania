using SystemZarzadzania.Setup;
using Api.Controllers;
using Api.Dto.AuthDtos;
using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Xunit;

namespace SystemZarzadzania.Tests;

public class UserTests : IClassFixture<TestFixture>, IAsyncLifetime
{
    private readonly TestFixture _fixture;

    public UserTests(TestFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync()
    { 
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _fixture.ResetDatabaseAsync();
    }

    [Fact]
    public async Task RegisterAndLoginUser_ShouldBeOk()
    {
        var result = await _fixture.RegisterAndLoginAsync();
        result.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task EmailAlreadyExists_ShouldBeConflict()
    {
        var result = await _fixture.RegisterAndLoginAsync();
        result.ShouldBe(HttpStatusCode.OK);

        var registerRequest = new RegisterDto
        {
            Name = "TesterName2",
            Lastname = "TesterLastname2",
            Password = "TesterPassword",
            ConfirmPassword = "TesterPassword",
            Email = "TesterEmail@Email.com",
            Phone = "123456789"
        };
        var registerResponse = await _fixture.Client.PostAsJsonAsync("api/auth/register", registerRequest);
        registerResponse.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
    
     [Fact]
    public async Task PasswordsDoesntMatch_ShouldBeBadRequest()
    {   
         var registerRequest = new RegisterDto
        {
            Name = "TesterName",
            Lastname = "TesterLastname",
            Password = "TesterPassword",
            ConfirmPassword = "TesterSecondPassword",
            Email = "TesterEmail@Email.com",
        };
        var registerResponse = await _fixture.Client.PostAsJsonAsync("api/auth/register", registerRequest);
        registerResponse.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

}