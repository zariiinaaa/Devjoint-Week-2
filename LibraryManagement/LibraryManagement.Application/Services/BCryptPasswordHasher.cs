using LibraryManagement.Core.Interfaces;
using BCryptNet = BCrypt.Net.BCrypt;

namespace LibraryManagement.Application.Services;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCryptNet.HashPassword(password);
    }

    public bool VerifyPassword(string password,string passwordHash)
    {
        return BCryptNet.Verify(password, passwordHash);
    }
}