using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Api.Dto.UserDtos;

public class UpdateUserDto
{
    public string Name { get; set; }

    public string Lastname { get; set; }

    [EmailAddress]
    public string Email { get; set; }
    public string Phone { get; set; }
}