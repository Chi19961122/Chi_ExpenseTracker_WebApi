using Chi.ExpenseTracker.Common.Models.InfoModels;
using Chi.ExpenseTracker.Common.Options;
using Chi.ExpenseTracker.Repositories.Entities;
using Chi.ExpenseTracker.Repositories.Users;
using Chi.ExpenseTracker.Services.Auth;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Chi.ExpenseTracker.Tests.Auth;

public class AuthServiceTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private AuthService CreateService()
    {
        var options = Options.Create(new JwtOptions
        {
            Key = "test-signing-key-that-is-long-enough-for-hs256-aaaa",
            Issuer = "test",
            Audience = "test",
            ExpiryMinutes = 60,
        });
        return new AuthService(_userRepository, options);
    }

    private static UserEntity CreateUser(string password = "demo1234") => new()
    {
        UserId = 1,
        UserName = "Test",
        Email = "test@example.com",
        Password = BCrypt.Net.BCrypt.HashPassword(password),
        Role = "User",
    };

    [Fact]
    public async Task LoginAsync_ReturnsTokenAndUser_WhenCredentialsValid()
    {
        _userRepository.GetByEmailAsync("test@example.com", Arg.Any<CancellationToken>())
            .Returns(CreateUser());
        var service = CreateService();

        var result = await service.LoginAsync("Test@Example.com ", "demo1234");

        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrWhiteSpace();
        result.User.UserId.Should().Be(1);
        result.User.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenPasswordWrong()
    {
        _userRepository.GetByEmailAsync("test@example.com", Arg.Any<CancellationToken>())
            .Returns(CreateUser());
        var service = CreateService();

        var result = await service.LoginAsync("test@example.com", "wrong-password");

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenUserNotFound()
    {
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((UserEntity?)null);
        var service = CreateService();

        var result = await service.LoginAsync("nobody@example.com", "demo1234");

        result.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_ReturnsNull_WhenEmailAlreadyExists()
    {
        _userRepository.GetByEmailAsync("test@example.com", Arg.Any<CancellationToken>())
            .Returns(CreateUser());
        var service = CreateService();

        var result = await service.RegisterAsync(new RegisterInfo
        {
            UserName = "Test",
            Email = "test@example.com",
            Password = "demo1234",
        });

        result.Should().BeNull();
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<UserEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterAsync_HashesPassword_AndDoesNotExposeIt()
    {
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((UserEntity?)null);
        UserEntity? saved = null;
        _userRepository.AddAsync(Arg.Do<UserEntity>(user => saved = user), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<UserEntity>());
        var service = CreateService();

        var result = await service.RegisterAsync(new RegisterInfo
        {
            UserName = "New",
            Email = "New@Example.com",
            Password = "demo1234",
        });

        result.Should().NotBeNull();
        result!.Email.Should().Be("new@example.com");
        saved.Should().NotBeNull();
        saved!.Password.Should().NotBe("demo1234");
        BCrypt.Net.BCrypt.Verify("demo1234", saved.Password).Should().BeTrue();
    }

    [Fact]
    public async Task RefreshAsync_IssuesNewToken_ForExistingUser()
    {
        _userRepository.GetByEmailAsync("test@example.com", Arg.Any<CancellationToken>())
            .Returns(CreateUser());
        var service = CreateService();

        var result = await service.RefreshAsync("test@example.com");

        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrWhiteSpace();
        result.User.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task RefreshAsync_ReturnsNull_WhenUserMissing()
    {
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((UserEntity?)null);
        var service = CreateService();

        var result = await service.RefreshAsync("gone@example.com");

        result.Should().BeNull();
    }
}
