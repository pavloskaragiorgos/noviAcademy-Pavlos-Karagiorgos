using Microsoft.Extensions.Logging;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Entities;

namespace WorldRank.Application.Services;

public class PlayerService
{
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(60);

    private readonly IPlayerRepository _playerRepository;
    private readonly ICache _cache;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(IPlayerRepository playerRepository, ICache cache, ILogger<PlayerService> logger)
    {
        _playerRepository = playerRepository;
        _cache = cache;
        _logger = logger;
    }

    private static string PlayerKey(int id) => $"player:{id}";
    private const string AllPlayersKey = "players:all";

    public Player AddPlayer(string name, int score)
    {
        var player = new Player(GeneratePlayerId(), name);
        player.AddScore(score);
        _playerRepository.AddPlayer(player);
        _logger.LogInformation("Player created {PlayerId} {Name} (score {Score})", player.Id, name, score);

        _cache.Set(PlayerKey(player.Id), player, Ttl);   // write-through
        _cache.Remove(AllPlayersKey);                     // invalidate the list
        _logger.LogInformation("Cache write-through player {PlayerId}; list cache invalidated", player.Id);
        return player;
    }

    public List<Player> ListPlayers()
    {
        if (_cache.TryGet(AllPlayersKey, out List<Player>? cached) && cached is not null)
        {
            _logger.LogInformation("Cache HIT  all players");
            return cached;
        }

        _logger.LogInformation("Cache MISS all players — loading from database");
        var players = _playerRepository.GetAllPlayers().ToList();
        _cache.Set(AllPlayersKey, players, Ttl);
        return players;
    }

    public List<IGrouping<int, Player>> ListPlayersByScore()
    {
        // Use the cached list of players to group by score and order by score descending
        return ListPlayers()
            .GroupBy(p => p.Score)
            .OrderByDescending(g => g.Key)
            .ToList();
    }

    public Player? FindPlayerByName(string name)
    {
        //Use the cached list of players to find a player by name 
        return ListPlayers()
            .FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public Player? FindPlayerById(int playerId)
    {
        if (_cache.TryGet(PlayerKey(playerId), out Player? cached) && cached is not null)
        {
            _logger.LogInformation("Cache HIT  player {PlayerId}", playerId);
            return cached;
        }

        _logger.LogInformation("Cache MISS player {PlayerId} — loading from database", playerId);
        var player = _playerRepository.FindPlayer(playerId);
        if (player is not null)
            _cache.Set(PlayerKey(playerId), player, Ttl);
        return player;
    }

    public void DeletePlayer(int playerId)
    {
        _playerRepository.DeletePlayer(playerId);
        _cache.Remove(PlayerKey(playerId));
        _cache.Remove(AllPlayersKey);
    }

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