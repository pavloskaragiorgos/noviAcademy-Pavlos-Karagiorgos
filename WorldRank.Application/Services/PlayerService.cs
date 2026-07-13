using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Entities;

namespace WorldRank.Application.Services;

public class PlayerService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(IPlayerRepository playerRepository, IMemoryCache cache, ILogger<PlayerService> logger)
    {
        _playerRepository = playerRepository;
        _cache = cache;
        _logger = logger;
    }

    public Player AddPlayer(string name, int score)
    {
        var player = new Player(GeneratePlayerId(), name);
        player.AddScore(score);
        _playerRepository.AddPlayer(player);
        return player;
    }

    public List<Player> ListPlayers()
    {
        if (_cache.TryGetValue("AllPlayersKey", out List<Player>? cached) && cached is not null)
        {
            _logger.LogInformation("Cache HIT  all players");
            return cached;
        }

        _logger.LogInformation("Cache MISS all players — loading from database");
        var players = _playerRepository.GetAllPlayers().ToList();

        _cache.Set("AllPlayersKey", players, TimeSpan.FromSeconds(60));

        return players;
    }

    public List<IGrouping<int, Player>> ListPlayersByScore()
    {
        var groups = _playerRepository.GroupPlayersByScore().ToList();

        return groups;
    }

    public Player? FindPlayerByName(string name)
    {
        return _playerRepository.GetAllPlayers()
            .FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public Player? FindPlayerById(int playerId)
    {
       return _playerRepository.FindPlayer(playerId);
    }

    public void DeletePlayer(int playerId)
    {
        _playerRepository.DeletePlayer(playerId);
    }

    // Generates a random, unique player id (avoids collisions with already-registered players).
    private int GeneratePlayerId()
    {
        var existingIds = _playerRepository.GetAllPlayers().Select(p => p.Id).ToHashSet();

        int id;
        do
        {
            id = Random.Shared.Next(1, int.MaxValue);
        }
        while (existingIds.Contains(id));

        return id;
    }

}
