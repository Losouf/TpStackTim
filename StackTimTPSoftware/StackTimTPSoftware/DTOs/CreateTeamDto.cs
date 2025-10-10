using System.ComponentModel.DataAnnotations;

namespace StackTimAPI.DTOs;

public class CreateTeamDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    [Required, StringLength(3, MinimumLength = 3)]
    public string Tag { get; set; } = null!;
}
