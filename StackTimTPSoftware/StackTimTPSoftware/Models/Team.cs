using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StackTimAPI.Models;

[Table("Teams")]
[Microsoft.EntityFrameworkCore.Index(nameof(Tag), IsUnique = true)]
public class Team
{
    [Key] public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required, StringLength(3, MinimumLength = 3)]
    public string Tag { get; set; } = null!;

    public int? CaptainId { get; set; }

    public Player? Captain { get; set; }
    public ICollection<TeamPlayer> TeamPlayers { get; set; } = new List<TeamPlayer>();
}
