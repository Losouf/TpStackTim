using Microsoft.AspNetCore.Mvc;
using StacktimApi.Tests.Helpers;
using StackTimAPI.Controllers;
using StackTimAPI.Db;
using StackTimAPI.DTOs;
using StackTimAPI.Models;
using Xunit;

namespace StacktimApi.Tests
{
    public class PlayersControllerTest : IDisposable
    {
        private readonly StackTimDbContext _db;
        private readonly PlayersController _controller;

        public PlayersControllerTest()
        {
            // Création du contexte via le helper
            _db = TestDbContextFactory.CreateInMemoryContext();
            _controller = new PlayersController(_db);
        }

        public void Dispose() => _db.Dispose();

        [Fact]
        public async Task GetPlayers_ReturnsAllPlayers()
        {
            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var players = Assert.IsAssignableFrom<IEnumerable<PlayerDto>>(okResult.Value);
            Assert.Equal(3, players.Count());
        }

        [Fact]
        public async Task GetPlayer_WithValidId_ReturnsPlayer()
        {
            // Arrange
            var firstPlayer = _db.Player.First();

            // Act
            var result = await _controller.GetById(firstPlayer.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var player = Assert.IsType<PlayerDto>(okResult.Value);
            Assert.Equal(firstPlayer.Pseudo, player.Pseudo);
        }

        [Fact]
        public async Task GetPlayer_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreatePlayer_WithValidData_ReturnsCreated()
        {
            // Arrange
            var dto = new CreatePlayerDto
            {
                Pseudo = "NewPlayer",
                Email = "new@mail.com",
                Rank = Rank.Argent
            };

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var player = Assert.IsType<PlayerDto>(created.Value);
            Assert.Equal("NewPlayer", player.Pseudo);
            Assert.Equal(0, player.TotalScore);
        }

        [Fact]
        public async Task CreatePlayer_WithDuplicatePseudo_ReturnsConflict()
        {
            // Arrange
            var existing = _db.Player.First();
            var dto = new CreatePlayerDto
            {
                Pseudo = existing.Pseudo,
                Email = "dup@mail.com",
                Rank = Rank.Bronze
            };

            // Act
            var result = await _controller.Create(dto);

            // Assert
            Assert.IsType<ConflictObjectResult>(result.Result);
        }

        [Fact]
        public async Task DeletePlayer_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var player = _db.Player.First();

            // Act
            var result = await _controller.Delete(player.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.False(_db.Player.Any(p => p.Id == player.Id));
        }

        [Fact]
        public async Task GetLeaderboard_ReturnsOrderedPlayers()
        {
            // Act
            var result = await _controller.Leaderboard();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var leaderboard = Assert.IsAssignableFrom<IEnumerable<PlayerDto>>(okResult.Value);

            // Vérifie que les scores sont bien triés dans l’ordre décroissant
            var scores = leaderboard.Select(p => p.TotalScore).ToList();
            var sortedScores = scores.OrderByDescending(s => s).ToList();
            Assert.Equal(sortedScores, scores);
        }
    }
}
