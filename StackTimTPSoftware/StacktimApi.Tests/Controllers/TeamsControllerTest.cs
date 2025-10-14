using Microsoft.AspNetCore.Mvc;
using StacktimApi.Tests.Helpers;
using StackTimAPI.Controllers;
using StackTimAPI.Db;
using StackTimAPI.DTOs;
using StackTimAPI.Models;
using Xunit;

namespace StacktimApi.Tests.Controllers
{
    public class TeamsControllerTest : IDisposable
    {
        private readonly StackTimDbContext _db;
        private readonly TeamsController _controller;

        public TeamsControllerTest()
        {
            _db = TestDbContextFactory.CreateInMemoryContext();
            _controller = new TeamsController(_db);
        }

        public void Dispose() => _db.Dispose();

        [Fact]
        public async Task AddPlayerToTeam_Ok_NoContent_And_LinkCreated()
        {
            var team = new Team { Name = "Alpha", Tag = "ALP" };
            _db.Teams.Add(team);
            await _db.SaveChangesAsync();

            var player = _db.Player.First();
            var dto = new AddPlayerToTeamDto { PlayerId = player.Id, Role = 1 };

            var res = await _controller.AddPlayerToTeam(team.Id, dto);

            Assert.IsType<NoContentResult>(res);
            Assert.True(_db.TeamPlayer.Any(tp => tp.TeamId == team.Id && tp.PlayerId == player.Id));
        }

        [Fact]
        public async Task AddPlayerToTeam_TeamNotFound_Returns404()
        {
            var anyPlayer = _db.Player.First();
            var dto = new AddPlayerToTeamDto { PlayerId = anyPlayer.Id };

            var res = await _controller.AddPlayerToTeam(999, dto);

            Assert.IsType<NotFoundObjectResult>(res);
        }

        [Fact]
        public async Task AddPlayerToTeam_PlayerNotFound_Returns404()
        {
            var team = new Team { Name = "Beta", Tag = "BET" };
            _db.Teams.Add(team);
            await _db.SaveChangesAsync();

            var dto = new AddPlayerToTeamDto { PlayerId = 99999 };

            var res = await _controller.AddPlayerToTeam(team.Id, dto);

            Assert.IsType<NotFoundObjectResult>(res);
        }

        [Fact]
        public async Task AddPlayerToTeam_AlreadyMember_ReturnsConflict()
        {
            var team = new Team { Name = "Gamma", Tag = "GAM" };
            _db.Teams.Add(team);
            await _db.SaveChangesAsync();

            var player = _db.Player.First();
            _db.TeamPlayer.Add(new TeamPlayer { TeamId = team.Id, PlayerId = player.Id, Role = 1 });
            await _db.SaveChangesAsync();

            var dto = new AddPlayerToTeamDto { PlayerId = player.Id, Role = 1 };

            var res = await _controller.AddPlayerToTeam(team.Id, dto);

            Assert.IsType<ConflictObjectResult>(res);
        }

        [Fact]
        public async Task RemovePlayerFromTeam_Ok_NoContent_And_LinkDeleted()
        {
            var team = new Team { Name = "Delta", Tag = "DEL" };
            _db.Teams.Add(team);
            await _db.SaveChangesAsync();

            var player = _db.Player.First();
            _db.TeamPlayer.Add(new TeamPlayer { TeamId = team.Id, PlayerId = player.Id, Role = 1 });
            await _db.SaveChangesAsync();

            var res = await _controller.RemovePlayerFromTeam(team.Id, player.Id);

            Assert.IsType<NoContentResult>(res);
            Assert.False(_db.TeamPlayer.Any(tp => tp.TeamId == team.Id && tp.PlayerId == player.Id));
        }

        [Fact]
        public async Task RemovePlayerFromTeam_TeamNotFound_Returns404()
        {
            var existingPlayer = _db.Player.First();

            var res = await _controller.RemovePlayerFromTeam(999, existingPlayer.Id);

            Assert.IsType<NotFoundObjectResult>(res);
        }

        [Fact]
        public async Task RemovePlayerFromTeam_PlayerNotFound_Returns404()
        {
            var team = new Team { Name = "Epsilon", Tag = "EPS" };
            _db.Teams.Add(team);
            await _db.SaveChangesAsync();

            var res = await _controller.RemovePlayerFromTeam(team.Id, 99999);

            Assert.IsType<NotFoundObjectResult>(res);
        }

        [Fact]
        public async Task RemovePlayerFromTeam_NotMember_Returns404()
        {
            var team = new Team { Name = "Zeta", Tag = "ZET" };
            _db.Teams.Add(team);
            await _db.SaveChangesAsync();

            var player = _db.Player.First();

            var res = await _controller.RemovePlayerFromTeam(team.Id, player.Id);

            Assert.IsType<NotFoundObjectResult>(res);
        }
    }
}
