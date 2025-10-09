using System.ComponentModel.DataAnnotations;
using StackTimAPI.Models;

namespace StackTimAPI.DTOs;

public class CreatePlayerDto
{
    [Required, StringLength(50)]
    public string Pseudo { get; set; } = null!;

    [Required, StringLength(100), EmailAddress]
    public string Email { get; set; } = null!;

    [Required, EnumDataType(typeof(Rank))]
    public Rank Rank { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalScore { get; set; } = 0;
}
