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

        public PlayersController(AppPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpPost]
        public IActionResult CreatePlayer([FromBody] CreatePlayerRequest request)
        {
            try
            {
                var player = _playerService.AddPlayer(request.Name, request.Score);
                return CreatedAtAction(nameof(GetPlayerById), new { playerId = player.Id }, player);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult ListPlayers()
        {
            try
            {
                var result = _playerService.ListPlayers();
                if (result == null) return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{playerId:int}")]
        public async Task<IActionResult> GetPlayerById(int playerId)
        {
            try
            {
                var player = _playerService.FindPlayerById(playerId);
                if (player == null) return NotFound();
                return Ok(player);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
                return StatusCode(500, ex.Message);
            }
        }

    }
}