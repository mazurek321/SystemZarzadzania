using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Api.Dto;

public class UpdateUserDto
{
    public string Name { get; set; }

    public string Lastname { get; set; }

    [EmailAddress]
    public string Email { get; set; }
}