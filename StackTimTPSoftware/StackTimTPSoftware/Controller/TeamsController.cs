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
            .Include(p => p.TeamPlayers)
            .ToListAsync();

        return Ok(teams);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Team>> GetById(int id)
    {
        if (!await _db.Teams.AnyAsync(t => t.Id == id))
            return NotFound("Équipe introuvable.");

        var team = await _db.Teams
            .Include(t => t.Captain)
            .Include(p => p.TeamPlayers)
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
        if (!await _db.Teams.AnyAsync(t => t.Id == id))
            return NotFound("Équipe introuvable.");

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

    [HttpPost("{teamId:int}/players")]
    public async Task<IActionResult> AddPlayerToTeam(int teamId, [FromBody] AddPlayerToTeamDto body)
    {
        if (!await _db.Teams.AnyAsync(t => t.Id == teamId))
            return NotFound("Équipe introuvable.");

        if (!await _db.Player.AnyAsync(p => p.Id == body.PlayerId))
            return NotFound("Joueur introuvable.");

        var existe = await _db.TeamPlayer.AnyAsync(tp => tp.TeamId == teamId && tp.PlayerId == body.PlayerId);
        if (existe)
            return Conflict("Ce joueur fait déjà partie de cette équipe.");

        _db.TeamPlayer.Add(new TeamPlayer
        {
            TeamId = teamId,
            PlayerId = body.PlayerId,
            Role = body.Role
        });

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{teamId:int}/players/{playerId:int}")]
    public async Task<IActionResult> RemovePlayerFromTeam(int teamId, int playerId)
    {
        if (!await _db.Teams.AnyAsync(t => t.Id == teamId))
            return NotFound("Équipe introuvable.");

        if (!await _db.Player.AnyAsync(p => p.Id == playerId))
            return NotFound("Joueur introuvable.");

        var lien = await _db.TeamPlayer.FindAsync(teamId, playerId);
        if (lien == null)
            return NotFound("Ce joueur n’appartient pas à cette équipe.");

        _db.TeamPlayer.Remove(lien);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
