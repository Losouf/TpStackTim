using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StackTimAPI.Models;

[Table("Players")]
public class Player
{
    [Key] public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Pseudo { get; set; } = null!;

    [Required, MaxLength(100)]
    public string Email { get; set; } = null!;

    [Required]
    public Rank Rank { get; set; }

    public int TotalScore { get; set; } = 0;

    public ICollection<TeamPlayer> TeamPlayers { get; set; } = new List<TeamPlayer>();
}
