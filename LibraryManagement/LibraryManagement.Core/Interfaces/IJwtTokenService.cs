using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Interfaces;

public interface IJwtTokenService
{
    (string AccessToken, DateTime ExpiresAt) GenerateToken(User user);
}