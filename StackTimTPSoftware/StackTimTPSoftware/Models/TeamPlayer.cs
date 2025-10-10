using System.ComponentModel.DataAnnotations.Schema;

namespace StackTimAPI.Models;

[Table("TeamPlayers")]
public class TeamPlayer
{
    public int TeamId { get; set; }
    public int PlayerId { get; set; }
    public int Role { get; set; }

    public Team Team { get; set; } = null!;
    public Player Player { get; set; } = null!;
}
