 namespace StackTimAPI.DTOs
{
    public class AddPlayerToTeamDto
    {
        public int PlayerId { get; set; }
        public int Role { get; set; } = 1; // 0=Capitaine, 1=Membre, 2=Remplaçant
    }
}
