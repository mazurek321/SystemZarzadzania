using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Api.Dto.AuthDtos;

public class RegisterDto
{
    [Required]
    public string Name { get; init; }

    [Required]
    public string Lastname { get; init; }

    [Required]
    public string Password { get; init; }

    [Required]
    [Compare("Password", ErrorMessage = "Passwords doesnt match.")]
    public string ConfirmPassword { get; init; }

    [Required]
    [EmailAddress]
    public string Email { get; init; }
    public string Phone { get; init; }
}