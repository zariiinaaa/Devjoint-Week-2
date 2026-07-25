using LibraryManagement.Core.DTOs;

namespace LibraryManagement.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);

    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
}