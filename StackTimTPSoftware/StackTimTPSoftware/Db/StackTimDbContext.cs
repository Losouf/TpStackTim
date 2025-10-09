using Microsoft.EntityFrameworkCore;
using StackTimAPI.Models;

namespace StackTimAPI.Db
{
    public class StackTimDbContext : DbContext
    {
        public StackTimDbContext(DbContextOptions<StackTimDbContext> options) : base(options) { }

        public System.Data.Entity.DbSet<Player> Player { get; set; }
        public System.Data.Entity.DbSet<TeamPlayer> TeamPlayer { get; set; }
        public System.Data.Entity.DbSet<Team> Teams { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Exemple : modelBuilder.Entity<Utilisateur>().Property(u => u.Email).IsRequired();
        }
    }
}
