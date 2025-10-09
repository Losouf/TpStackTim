using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackTimAPI.Db;
using StackTimAPI.DTOs;
using StackTimAPI.Models;

namespace StackTimAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly StackTimDbContext _db;
    public PlayersController(StackTimDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAll()
    {
        var data = await _db.Player
            .AsNoTracking()
            .Select(p => new PlayerDto
            {
                Id = p.Id,
                Pseudo = p.Pseudo,
                Email = p.Email,
                Rank = p.Rank,
                TotalScore = p.TotalScore
            })
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PlayerDto>> GetById(int id)
    {
        var p = await _db.Player
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new PlayerDto
            {
                Id = x.Id,
                Pseudo = x.Pseudo,
                Email = x.Email,
                Rank = x.Rank,
                TotalScore = x.TotalScore
            })
            .FirstOrDefaultAsync();

        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<ActionResult<PlayerDto>> Create([FromBody] CreatePlayerDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        if (await _db.Player.AnyAsync(p => p.Pseudo == dto.Pseudo))
            return Conflict(new { message = "Pseudo déjà utilisé." });

        if (await _db.Player.AnyAsync(p => p.Email == dto.Email))
            return Conflict(new { message = "Email déjà utilisé." });

        var entity = new Player
        {
            Pseudo = dto.Pseudo,
            Email = dto.Email,
            Rank = dto.Rank,
            TotalScore = 0
        };

        _db.Player.Add(entity);
        await _db.SaveChangesAsync();

        var result = new PlayerDto
        {
            Id = entity.Id,
            Pseudo = entity.Pseudo,
            Email = entity.Email,
            Rank = entity.Rank,
            TotalScore = entity.TotalScore
        };

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PlayerDto>> Update(int id, [FromBody] UpdatePlayerDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var p = await _db.Player.FirstOrDefaultAsync(x => x.Id == id);
        if (p is null) return NotFound();

        if (dto.Pseudo != null && dto.Pseudo != p.Pseudo)
        {
            var pseudoUsed = await _db.Player.AnyAsync(x => x.Pseudo == dto.Pseudo && x.Id != id);
            if (pseudoUsed) return Conflict(new { message = "Pseudo déjà utilisé." });
            p.Pseudo = dto.Pseudo;
        }

        if (dto.Email != null && dto.Email != p.Email)
        {
            var emailUsed = await _db.Player.AnyAsync(x => x.Email == dto.Email && x.Id != id);
            if (emailUsed) return Conflict(new { message = "Email déjà utilisé." });
            p.Email = dto.Email;
        }

        if (dto.Rank.HasValue) p.Rank = dto.Rank.Value;
        if (dto.TotalScore.HasValue) p.TotalScore = dto.TotalScore.Value;

        await _db.SaveChangesAsync();

        var result = new PlayerDto
        {
            Id = p.Id,
            Pseudo = p.Pseudo,
            Email = p.Email,
            Rank = p.Rank,
            TotalScore = p.TotalScore
        };
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Player.FindAsync(id);
        if (p is null) return NotFound();

        _db.Player.Remove(p);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("leaderboard")]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> Leaderboard()
    {
        var top = await _db.Player
            .AsNoTracking()
            .OrderByDescending(p => p.TotalScore)
            .ThenBy(p => p.Id)
            .Take(10)
            .Select(p => new PlayerDto
            {
                Id = p.Id,
                Pseudo = p.Pseudo,
                Email = p.Email,
                Rank = p.Rank,
                TotalScore = p.TotalScore
            })
            .ToListAsync();

        return Ok(top);
    }
}
