using WorldRank.Domain.Entities;

namespace WorldRank.Application.Interfaces;

public interface IPlayerRepository
{
    Task AddPlayerAsync(Player player, CancellationToken cancellationToken = default);

    Task<IEnumerable<Player>> GetAllPlayersAsync(CancellationToken cancellationToken = default);

    Task DeletePlayerAsync(int playerId, CancellationToken cancellationToken = default);

    Task<Player?> FindPlayerAsync(int playerId, CancellationToken cancellationToken = default);

    Task<IEnumerable<IGrouping<int, Player>>> GroupPlayersByScoreAsync(CancellationToken cancellationToken = default);
}