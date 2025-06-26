using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Api.Dto.AuthDtos;

public class RegisterDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Lastname { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "Passwords doesnt match.")]
    public string ConfirmPassword { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
    public string Phone { get; set; }
}