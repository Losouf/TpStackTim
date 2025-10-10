using Microsoft.EntityFrameworkCore;
using StackTimAPI.Db;
using StackTimAPI.Models;

namespace StacktimApi.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static StackTimDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<StackTimDbContext>()
                .UseInMemoryDatabase(databaseName: $"StackTimTestDb_{Guid.NewGuid()}")
                .Options;

            var context = new StackTimDbContext(options);

            if (!context.Player.Any())
            {
                context.Player.AddRange(
                    new Player { Pseudo = "PlayerOne", Email = "one@mail.com", Rank = Rank.Or, TotalScore = 100 },
                    new Player { Pseudo = "PlayerTwo", Email = "two@mail.com", Rank = Rank.Bronze, TotalScore = 50 },
                    new Player { Pseudo = "PlayerThree", Email = "three@mail.com", Rank = Rank.Platine, TotalScore = 200 }
                );

                context.SaveChanges();
            }

            return context;
        }
    }
}
