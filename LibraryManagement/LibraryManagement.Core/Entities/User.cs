using System.ComponentModel.DataAnnotations;
using LibraryManagement.Core.Common;

namespace LibraryManagement.Core.Entities;

public class User : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = UserRoles.User;
}