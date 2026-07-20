using LibraryManagement.Application.Services;
using Xunit;

namespace LibraryManagement.Tests;

public class BCryptPasswordHasherTests
{
    [Fact]
    public void HashPassword_WhenCalled_CreatesValidHash()
    {

        var passwordHasher = new BCryptPasswordHasher();
        var password = "Test123!";


        var passwordHash =
            passwordHasher.HashPassword(password);
        Assert.NotEqual(password, passwordHash);

        Assert.True(
            passwordHasher.VerifyPassword(
                password,
                passwordHash));

        Assert.False(
            passwordHasher.VerifyPassword(
                "WrongPassword",
                passwordHash));
    }
}