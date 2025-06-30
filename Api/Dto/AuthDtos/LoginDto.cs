using System.ComponentModel.DataAnnotations;

namespace Api.Dto.AuthDtos;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; init; }

    [Required]
    public string Password { get; init; }
}