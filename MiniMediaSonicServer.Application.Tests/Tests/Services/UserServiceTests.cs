using AwesomeAssertions;
using BaseTests;
using BaseTests.Attributes;
using BaseTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Application.Tests.Tests.Services;

public class UserServiceTests : IntegrationTest
{
    public UserServiceTests(ApiWebApplicationFactory fixture)
        : base(fixture)
    {
        
    }

    [Fact]
    public async Task GetUserByUsername_Ok()
    {
        using var scope = _factory.Services.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<UserService>();
        var user = await userService.GetUserByUsernameAsync(_authConfig.Username);
        user.Should().NotBeNull();
    }

    [Fact]
    public async Task ValidateToken_Ok()
    {
        using var scope = _factory.Services.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<UserService>();
        var user = await userService.GetUserByUsernameAsync(_authConfig.Username);
        user.Should().NotBeNull();
        var authed = userService.ValidateToken(user.TokenBasedAuth, _authConfig.Token, _authConfig.Salt);
        authed.Should().BeTrue();
    }

    [Theory, Repeat(1000)]
    public async Task FuzzUserLogin(string args)
    {
        string username = BogusHelper.GetrandomString(1, 100);
        using var scope = _factory.Services.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<UserService>();
        var user = await userService.GetUserByUsernameAsync(username);
        user.Should().BeNull();
    }

    [Theory, Repeat(1000)]
    public async Task FuzzValidateToken(string args)
    {
        string password = BogusHelper.GetrandomString(1, 100);
        string token = BogusHelper.GetrandomString(1, 100);
        string salt = BogusHelper.GetrandomString(1, 100);
        using var scope = _factory.Services.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<UserService>();
        var user = userService.ValidateToken(password, token, salt);
        user.Should().BeFalse();
    }

    [Theory, Repeat(1000)]
    public async Task FuzzValidatePassword(string args)
    {
        string password = BogusHelper.GetrandomString(1, 100);
        string hashedPassword = BogusHelper.GetrandomString(1, 100);
        using var scope = _factory.Services.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<UserService>();
        var user = userService.ValidatePassword(password, hashedPassword);
        user.Should().BeFalse();
    }
}