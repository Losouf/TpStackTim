using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StacktimApi.Tests.Helpers;
using StackTimAPI.Controllers;
using StackTimAPI.Db;
using StackTimAPI.DTOs;
using StackTimAPI.Models;
using Xunit;

namespace StacktimApi.Tests.Controllers
{
    public class PlayersControllerTest : IDisposable
    {
        private readonly StackTimDbContext _db;
        private readonly PlayersController _controller;

        public PlayersControllerTest()
        {
            _db = TestDbContextFactory.CreateInMemoryContext();
            _controller = new PlayersController(_db);
        }

        public void Dispose() => _db.Dispose();

        [Fact]
        public async Task GetPlayers_ReturnsAllPlayers()
        {
            var result = await _controller.GetAll();
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var players = Assert.IsAssignableFrom<IEnumerable<PlayerDto>>(ok.Value);
            Assert.Equal(3, players.Count());
        }

        [Fact]
        public async Task GetPlayer_WithValidId_ReturnsPlayer()
        {
            var first = _db.Player.First();
            var result = await _controller.GetById(first.Id);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<PlayerDto>(ok.Value);
            Assert.Equal(first.Pseudo, dto.Pseudo);
        }

        [Fact]
        public async Task GetPlayer_WithInvalidId_ReturnsNotFound()
        {
            var result = await _controller.GetById(999);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreatePlayer_WithValidData_ReturnsCreated()
        {
            var dto = new CreatePlayerDto { Pseudo = "NewPlayer", Email = "new@mail.com", Rank = Rank.Argent };
            var result = await _controller.Create(dto);
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var player = Assert.IsType<PlayerDto>(created.Value);
            Assert.Equal("NewPlayer", player.Pseudo);
            Assert.Equal(0, player.TotalScore);
        }

        [Fact]
        public async Task CreatePlayer_WithDuplicatePseudo_ReturnsConflict()
        {
            var existing = _db.Player.First();
            var dto = new CreatePlayerDto { Pseudo = existing.Pseudo, Email = "dup@mail.com", Rank = Rank.Bronze };
            var result = await _controller.Create(dto);
            Assert.IsType<ConflictObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeletePlayer_WithValidId_ReturnsNoContent()
        {
            var player = _db.Player.First();
            var result = await _controller.Delete(player.Id);
            Assert.IsType<NoContentResult>(result);
            Assert.False(_db.Player.Any(p => p.Id == player.Id));
        }

        [Fact]
        public async Task GetLeaderboard_ReturnsOrderedPlayers()
        {
            var result = await _controller.Leaderboard();
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<PlayerDto>>(ok.Value);
            var scores = list.Select(p => p.TotalScore).ToList();
            Assert.Equal(scores.OrderByDescending(s => s).ToList(), scores);
        }

        [Fact]
        public async Task CreatePlayer_WithDuplicateEmail_ReturnsConflict()
        {
            var existing = _db.Player.First();
            var dto = new CreatePlayerDto { Pseudo = "SomeoneNew", Email = existing.Email, Rank = Rank.Bronze };
            var result = await _controller.Create(dto);
            Assert.IsType<ConflictObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdatePlayer_WithDuplicateEmail_ReturnsConflict()
        {
            var p1 = _db.Player.First();
            var p2 = _db.Player.Skip(1).First();
            var dto = new UpdatePlayerDto { Email = p1.Email };
            var result = await _controller.Update(p2.Id, dto);
            Assert.IsType<ConflictObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdatePlayer_WithSameValues_NoChange_ReturnsOk()
        {
            var p = _db.Player.First();
            var dto = new UpdatePlayerDto { Pseudo = p.Pseudo, Email = p.Email, Rank = p.Rank, TotalScore = p.TotalScore };
            var result = await _controller.Update(p.Id, dto);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var updated = Assert.IsType<PlayerDto>(ok.Value);
            Assert.Equal(p.Pseudo, updated.Pseudo);
            Assert.Equal(p.Email, updated.Email);
            Assert.Equal(p.Rank, updated.Rank);
            Assert.Equal(p.TotalScore, updated.TotalScore);
        }

        [Fact]
        public async Task UpdatePlayer_ChangeOnlyPseudo_Succeeds()
        {
            var p = _db.Player.First();

            // pseudo unique (évite les collisions entre runs)
            var newPseudo = "p_" + Guid.NewGuid().ToString("N").Substring(6);

            var result = await _controller.Update(p.Id, new UpdatePlayerDto { Pseudo = newPseudo });

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var updated = Assert.IsType<PlayerDto>(ok.Value);
            Assert.Equal(newPseudo, updated.Pseudo);
        }


        [Fact]
        public async Task UpdatePlayer_NotFound_Returns404()
        {
            var res = await _controller.Update(99999, new UpdatePlayerDto { Pseudo = "x" });
            Assert.IsType<NotFoundResult>(res.Result);
        }

        [Fact]
        public async Task UpdatePlayer_DuplicatePseudo_ReturnsConflict()
        {
            var p1 = _db.Player.First();
            var p2 = _db.Player.Skip(1).First();
            var res = await _controller.Update(p2.Id, new UpdatePlayerDto { Pseudo = p1.Pseudo });
            Assert.IsType<ConflictObjectResult>(res.Result);
        }

        [Fact]
        public async Task UpdatePlayer_Partial_Rank_And_TotalScore_Works()
        {
            var p = _db.Player.First();
            var res = await _controller.Update(p.Id, new UpdatePlayerDto { Rank = Rank.Diamant, TotalScore = 999 });
            var ok = Assert.IsType<OkObjectResult>(res.Result);
            var dto = Assert.IsType<PlayerDto>(ok.Value);
            Assert.Equal(Rank.Diamant, dto.Rank);
            Assert.Equal(999, dto.TotalScore);
        }

        [Fact]
        public async Task DeletePlayer_NotFound_Returns404()
        {
            var res = await _controller.Delete(88888);
            Assert.IsType<NotFoundResult>(res);
        }

        [Fact]
        public async Task Leaderboard_Empty_ReturnsEmpty()
        {
            var opt = new DbContextOptionsBuilder<StackTimDbContext>()
                .UseInMemoryDatabase($"Empty_{Guid.NewGuid()}")
                .Options;
            using var emptyDb = new StackTimDbContext(opt);
            var ctrl = new PlayersController(emptyDb);

            var res = await ctrl.Leaderboard();
            var ok = Assert.IsType<OkObjectResult>(res.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<PlayerDto>>(ok.Value);
            Assert.Empty(data);
        }
    }
}
