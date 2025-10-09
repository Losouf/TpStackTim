using StackTimAPI.Models;

public class PlayerDto
{
    public int Id { get; set; }
    public string Pseudo { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Rank Rank { get; set; }
    public int TotalScore { get; set; }
}