using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackTimAPI.Db;
using StackTimAPI.DTOs;
using StackTimAPI.Models;

namespace StackTimAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly StackTimDbContext _db;
    public TeamsController(StackTimDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Team>>> GetAll()
    {
        var teams = await _db.Teams
            .Include(t => t.Captain)
            .ToListAsync();

        return Ok(teams);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Team>> GetById(int id)
    {
        var team = await _db.Teams
            .Include(t => t.Captain)
            .FirstOrDefaultAsync(t => t.Id == id);

        return team is null ? NotFound() : Ok(team);
    }

    [HttpPost]
    public async Task<ActionResult<Team>> Create(CreateTeamDto dto)
    {
        if (await _db.Teams.AnyAsync(t => t.Name == dto.Name))
            return BadRequest("Nom déjà utilisé.");

        if (await _db.Teams.AnyAsync(t => t.Tag == dto.Tag))
            return BadRequest("Tag déjà utilisé.");

        var team = new Team
        {
            Name = dto.Name,
            Tag = dto.Tag,
            CaptainId = dto.CaptainId
        };

        _db.Teams.Add(team);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = team.Id }, team);
    }

    [HttpGet("{id}/roster")]
    public async Task<ActionResult> GetRoster(int id)
    {
        var team = await _db.Teams.Include(t => t.TeamPlayers)
                                  .ThenInclude(tp => tp.Player)
                                  .FirstOrDefaultAsync(t => t.Id == id);
        if (team == null) return NotFound();

        var roster = team.TeamPlayers.Select(tp => new
        {
            tp.PlayerId,
            tp.Player.Pseudo,
            tp.Role
        });

        return Ok(roster);
    }
}
