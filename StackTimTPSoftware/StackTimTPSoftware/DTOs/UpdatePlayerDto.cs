using System.ComponentModel.DataAnnotations;
using StackTimAPI.Models;

namespace StackTimAPI.DTOs;

public class UpdatePlayerDto
{
    [StringLength(50)]
    public string? Pseudo { get; set; }

    [StringLength(100), EmailAddress]
    public string? Email { get; set; }

    [EnumDataType(typeof(Rank))]
    public Rank? Rank { get; set; }

    [Range(0, int.MaxValue)]
    public int? TotalScore { get; set; }
}
