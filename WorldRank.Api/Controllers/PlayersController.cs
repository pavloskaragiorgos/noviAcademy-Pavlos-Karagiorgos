using Microsoft.AspNetCore.Mvc;
using WorldRank.Api.Dtos;
using WorldRank.Application.Services;
using WorldRank.Domain.Entities;
using AppPlayerService = WorldRank.Application.Services.PlayerService;

namespace WorldRank.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly AppPlayerService _playerService;
        private readonly ILogger<PlayersController> _logger;

        public PlayersController(AppPlayerService playerService, ILogger<PlayersController> logger)
        {
            _playerService = playerService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var player = await _playerService.AddPlayerAsync(request.Name, request.Score, cancellationToken);
                return CreatedAtAction(nameof(GetPlayerById), new { playerId = player.Id }, player);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating player");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListPlayers(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _playerService.ListPlayersAsync(cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error listing players");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("by-score")]
        public async Task<IActionResult> ListPlayersByScore(CancellationToken cancellationToken)
        {
            try
            {
                var groups = await _playerService.ListPlayersByScoreAsync(cancellationToken);
                var result = groups.Select(g => new PlayerScoreGroup(g.Key, g.ToList())).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error listing players by score");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> FindPlayerByName([FromQuery] string name, CancellationToken cancellationToken)
        {
            try
            {
                var player = await _playerService.FindPlayerByNameAsync(name, cancellationToken);
                if (player is null) return NotFound();
                return Ok(player);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error searching player by name {Name}", name);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("{playerId:int}")]
        public async Task<IActionResult> GetPlayerById(int playerId, CancellationToken cancellationToken)
        {
            try
            {
                var player = await _playerService.FindPlayerByIdAsync(playerId, cancellationToken);
                if (player == null) return NotFound();
                return Ok(player);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching player {PlayerId}", playerId);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpDelete("{playerId:int}")]
        public async Task<IActionResult> DeletePlayer(int playerId, CancellationToken cancellationToken)
        {
            try
            {
                await _playerService.DeletePlayerAsync(playerId, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting player {PlayerId}", playerId);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}