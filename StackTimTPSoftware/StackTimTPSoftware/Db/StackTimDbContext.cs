using Microsoft.EntityFrameworkCore;
using StackTimAPI.Models;

namespace StackTimAPI.Db
{
    public class StackTimDbContext : DbContext
    {
        public StackTimDbContext(DbContextOptions<StackTimDbContext> options) : base(options) { }

        public DbSet<Player> Player { get; set; }
        public DbSet<TeamPlayer> TeamPlayer { get; set; }
        public DbSet<Team> Teams { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Exemple : modelBuilder.Entity<Utilisateur>().Property(u => u.Email).IsRequired();
            modelBuilder.Entity<TeamPlayer>()
                .HasKey(tp => new { tp.TeamId, tp.PlayerId });

            modelBuilder
                .Entity<Player>()
                .Property(p => p.Rank)
                .HasConversion<string>();
        }
    }
}
