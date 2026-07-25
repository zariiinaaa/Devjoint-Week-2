using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers;

/// <summary>
/// Handles user registration and authentication.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user and returns a JWT access token.
    /// </summary>
    /// <param name="dto">New user registration information.</param>
    [HttpPost("register")]
    [ProducesResponseType(
        typeof(AuthResponseDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto dto)
    {
        var response =await _authService.RegisterAsync(dto);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT access token.
    /// </summary>
    /// <param name="dto">User login information.</param>
    [HttpPost("login")]
    [ProducesResponseType(
        typeof(AuthResponseDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto dto)
    {
        var response =await _authService.LoginAsync(dto);

        return Ok(response);
    }
}