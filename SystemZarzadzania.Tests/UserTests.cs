using SystemZarzadzania.Setup;
using Api.Dto.AuthDtos;
using Api.Dto.UserDtos;
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

    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();
    }


    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task RegisterAndLoginUser_ShouldBeOk()
    {
        var request = await _fixture.RegisterAndLoginAsync();
        request.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task EmailAlreadyExists_ShouldBeConflict()
    {
        var request = await _fixture.RegisterAndLoginAsync();
        request.ShouldBe(HttpStatusCode.OK);

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

    [Fact]
    public async Task WrongEmailWhileLogging_ShouldBeNotFound()
    {
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
        registerResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var loginRequest = new LoginDto
        {
            Email = "NotExistingEmail@wp.pl",
            Password = "TesterPassword"
        };

        var loginResponse = await _fixture.Client.PostAsJsonAsync("api/auth/login", loginRequest);
        loginResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WrongPasswordWhileLogging_ShouldBeUnauthorized()
    {
        var registerRequest = new RegisterDto
        {
            Name = "TesterName",
            Lastname = "TesterLastname",
            Password = "TesterPassword",
            ConfirmPassword = "TesterPassword",
            Email = "TesterEmail@Email.com",
            Phone = "123456789"
        };
        var registerResponse = await _fixture.Client.PostAsJsonAsync("api/auth/register", registerRequest);
        registerResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var loginRequest = new LoginDto
        {
            Email = "TesterEmail@Email.com",
            Password = "TesterSecondPassword"
        };

        var loginResponse = await _fixture.Client.PostAsJsonAsync("api/auth/login", loginRequest);
        loginResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCurrentUser_ShouldReturnOk()
    {
        await _fixture.RegisterAndLoginAsync();

        var response = await _fixture.Client.GetAsync("api/user");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Email.ShouldBe("TesterEmail@Email.com");
    }

    [Fact]
    public async Task GetUser_ShouldReturnOk()
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
        var registerResponseContent = await registerResponse.Content.ReadAsStringAsync();

        await _fixture.RegisterAndLoginAsync();

        var response = await _fixture.Client.GetAsync($"api/user?userId={registerResponseContent}");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUser_ShouldReturnNotFound()
    {
        await _fixture.RegisterAndLoginAsync();

        var id = Guid.NewGuid();

        var response = await _fixture.Client.GetAsync($"api/user?userId={id}");
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnOk()
    {
        await _fixture.RegisterAndLoginAsync();

        var request = new UpdateUserDto
        {
            Name = "TesterX",
            Lastname = "TesterX",
            Email = "TesterX@Email.com",
            Phone = "987654321"
        };

        var response = await _fixture.Client.PutAsJsonAsync("api/user", request);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var updatedUser = await response.Content.ReadFromJsonAsync<UserDto>();
        updatedUser.Email.ShouldBe(request.Email);
    }

    [Fact]
    public async Task UpdatePassword_ShouldReturnOk()
    {
        await _fixture.RegisterAndLoginAsync();

        var request = new UpdateUserPasswordDto
        {
            OldPassword = "TesterPassword",
            NewPassword = "TesterNewPasswordX",
            ConfirmNewPassword = "TesterNewPasswordX"
        };

        var response = await _fixture.Client.PutAsJsonAsync("api/user/password", request);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PasswordsDoesntMatch_ShouldReturnBadRequest()
    {
        await _fixture.RegisterAndLoginAsync();

        var request = new UpdateUserPasswordDto
        {
            OldPassword = "TesterPassword",
            NewPassword = "TesterNewPasswordX",
            ConfirmNewPassword = "TesterNewNewPasswordX"
        };

        var response = await _fixture.Client.PutAsJsonAsync("api/user/password", request);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WrongOldPassword_ShouldReturnBadRequest()
    {
        await _fixture.RegisterAndLoginAsync();

        var request = new UpdateUserPasswordDto
        {
            OldPassword = "TesterBadPassword",
            NewPassword = "TesterNewPasswordX",
            ConfirmNewPassword = "TesterNewPasswordX"
        };

        var response = await _fixture.Client.PutAsJsonAsync("api/user/password", request);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnNoContent()
    {
        await _fixture.RegisterAndLoginAsync();

        var request = await _fixture.Client.DeleteAsync("api/user");
        request.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}