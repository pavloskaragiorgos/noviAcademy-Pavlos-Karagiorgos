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

		public void AddPlayer(Player player)
		{
			_players.Add(player);
			_logger.LogInformation("Player {PlayerId} ({Name}) added with score {Score}", player.Id, player.Name, player.Score);
		}

		public IEnumerable<Player> GetAllPlayers()
		{
			// Return a copy so callers cannot mutate the repository's internal list.
			return _players.ToList();
		}

		public void DeletePlayer(int playerId)
		{
			var player = _players.Where(item => item.Id == playerId).FirstOrDefault();

			if (player is null)
			{
				_logger.LogWarning("Delete skipped: player {PlayerId} not found", playerId);
				return;
			}

			_players.Remove(player);
			_logger.LogInformation("Player {PlayerId} deleted", playerId);
		}

		public Player? FindPlayer(int playerId)
		{
			return _players.Where(item => item.Id == playerId).FirstOrDefault();
		}

		public IEnumerable<IGrouping<int, Player>> GroupPlayersByScore()
		{
			return _players
				.GroupBy(player => player.Score)
				.OrderByDescending(group => group.Key);
		}
	}
}
