using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Api.Dto.UserDtos;

public class UpdateUserPasswordDto
{
    [Required]
    public string OldPassword { get; set; }
    [Required]
    public string NewPassword { get; set; }

    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords doesnt match.")]
    public string ConfirmNewPassword { get; set; }
}