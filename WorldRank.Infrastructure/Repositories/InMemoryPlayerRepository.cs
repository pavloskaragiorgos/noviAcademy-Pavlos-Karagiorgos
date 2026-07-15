using Microsoft.Extensions.Logging;
using WorldRank.Domain.Entities;
using WorldRank.Application.Interfaces;

namespace WorldRank.Infrastructure.Repositories
{
    public class InMemoryPlayerRepository : IPlayerRepository
    {
        private readonly ILogger<InMemoryPlayerRepository> _logger;

        private List<Player> _players = new();

        public InMemoryPlayerRepository(ILogger<InMemoryPlayerRepository> logger)
        {
            _logger = logger;
        }

        public Task AddPlayerAsync(Player player, CancellationToken cancellationToken = default)
        {
            _players.Add(player);
            _logger.LogInformation("Player {PlayerId} ({Name}) added with score {Score}", player.Id, player.Name, player.Score);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Player>> GetAllPlayersAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<Player>>(_players.ToList());
        }

        public Task DeletePlayerAsync(int playerId, CancellationToken cancellationToken = default)
        {
            var player = _players.Where(item => item.Id == playerId).FirstOrDefault();

            if (player is null)
            {
                _logger.LogWarning("Delete skipped: player {PlayerId} not found", playerId);
                return Task.CompletedTask;
            }

            _players.Remove(player);
            _logger.LogInformation("Player {PlayerId} deleted", playerId);
            return Task.CompletedTask;
        }

        public Task<Player?> FindPlayerAsync(int playerId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_players.Where(item => item.Id == playerId).FirstOrDefault());
        }

        public Task<IEnumerable<IGrouping<int, Player>>> GroupPlayersByScoreAsync(CancellationToken cancellationToken = default)
        {
            var groups = _players
                .GroupBy(player => player.Score)
                .OrderByDescending(group => group.Key);
            return Task.FromResult<IEnumerable<IGrouping<int, Player>>>(groups);
        }
    }
}