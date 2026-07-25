using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Core.DTOs;

public class RegisterRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    [Required]
    [MinLength(6)]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}