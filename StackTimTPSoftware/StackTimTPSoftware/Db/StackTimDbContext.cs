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

            modelBuilder.Entity<Player>().ToTable("Players");
            modelBuilder.Entity<Team>().ToTable("Teams");
            modelBuilder.Entity<TeamPlayer>().ToTable("TeamPlayers");

            modelBuilder.Entity<TeamPlayer>(e =>
            {
                e.HasKey(tp => new { tp.TeamId, tp.PlayerId });

                e.Property(tp => tp.TeamId).HasColumnName("TeamId");
                e.Property(tp => tp.PlayerId).HasColumnName("PlayerId");
                e.Property(tp => tp.Role).HasColumnName("Role");

                e.HasOne(tp => tp.Team)
                 .WithMany(t => t.TeamPlayers)
                 .HasForeignKey(tp => tp.TeamId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(tp => tp.Player)
                 .WithMany(p => p.TeamPlayers)
                 .HasForeignKey(tp => tp.PlayerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Player>()
                .Property(p => p.Rank)
                .HasConversion<string>()
                .HasMaxLength(20);

        }

    }
}
