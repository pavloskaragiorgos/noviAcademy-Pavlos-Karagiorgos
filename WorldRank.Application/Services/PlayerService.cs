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

    public async Task<Player> AddPlayerAsync(string name, int score, CancellationToken cancellationToken = default)
    {
        var player = new Player(await GeneratePlayerIdAsync(cancellationToken), name);
        player.AddScore(score);
        await _playerRepository.AddPlayerAsync(player, cancellationToken);
        _logger.LogInformation("Player created {PlayerId} {Name} (score {Score})", player.Id, name, score);

        _cache.Set(PlayerKey(player.Id), player, Ttl);
        _cache.Remove(AllPlayersKey);
        _logger.LogInformation("Cache write-through player {PlayerId}; list cache invalidated", player.Id);
        return player;
    }

    public async Task<List<Player>> ListPlayersAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGet(AllPlayersKey, out List<Player>? cached) && cached is not null)
        {
            _logger.LogInformation("Cache HIT  all players");
            return cached;
        }

        _logger.LogInformation("Cache MISS all players — loading from database");
        var players = (await _playerRepository.GetAllPlayersAsync(cancellationToken)).ToList();
        _cache.Set(AllPlayersKey, players, Ttl);
        return players;
    }

    public async Task<List<IGrouping<int, Player>>> ListPlayersByScoreAsync(CancellationToken cancellationToken = default)
    {
        return (await ListPlayersAsync(cancellationToken))
            .GroupBy(p => p.Score)
            .OrderByDescending(g => g.Key)
            .ToList();
    }

    public async Task<Player?> FindPlayerByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return (await ListPlayersAsync(cancellationToken))
            .FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<Player?> FindPlayerByIdAsync(int playerId, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGet(PlayerKey(playerId), out Player? cached) && cached is not null)
        {
            _logger.LogInformation("Cache HIT  player {PlayerId}", playerId);
            return cached;
        }

        _logger.LogInformation("Cache MISS player {PlayerId} — loading from database", playerId);
        var player = await _playerRepository.FindPlayerAsync(playerId, cancellationToken);
        if (player is not null)
            _cache.Set(PlayerKey(playerId), player, Ttl);
        return player;
    }

    public async Task DeletePlayerAsync(int playerId, CancellationToken cancellationToken = default)
    {
        await _playerRepository.DeletePlayerAsync(playerId, cancellationToken);
        _cache.Remove(PlayerKey(playerId));
        _cache.Remove(AllPlayersKey);
    }

    private async Task<int> GeneratePlayerIdAsync(CancellationToken cancellationToken)
    {
        var existingIds = (await _playerRepository.GetAllPlayersAsync(cancellationToken)).Select(p => p.Id).ToHashSet();

        int id;
        do
        {
            id = Random.Shared.Next(1, int.MaxValue);
        }
        while (existingIds.Contains(id));

        return id;
    }
}