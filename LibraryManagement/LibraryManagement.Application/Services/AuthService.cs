using LibraryManagement.Core.Common;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;

namespace LibraryManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var username = dto.Username.Trim();
        var email = dto.Email.Trim().ToLowerInvariant();

        var usernameExists =
            await _userRepository.UsernameExistsAsync(username);

        if (usernameExists)
        {
            throw new InvalidOperationException(
                "A user with this username already exists.");
        }

        var emailExists =
            await _userRepository.EmailExistsAsync(email);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash =_passwordHasher.HashPassword(dto.Password),
            Role = UserRoles.User
        };

        var createdUser =
            await _userRepository.CreateAsync(user);

        return CreateAuthResponse(createdUser);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var user =
            await _userRepository.GetByEmailAsync(email);

        if (user is null ||
            !_passwordHasher.VerifyPassword(
                dto.Password,
                user.PasswordHash))
        {
            throw new UnauthorizedAccessException(
                "Email or password is incorrect.");
        }

        return CreateAuthResponse(user);
    }

    private AuthResponseDto CreateAuthResponse(User user)
    {
        var (accessToken, expiresAt) =
            _jwtTokenService.GenerateToken(user);

        return new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            AccessToken = accessToken,
            ExpiresAt = expiresAt
        };
    }
}