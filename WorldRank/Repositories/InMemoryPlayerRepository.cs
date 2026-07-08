using System.Runtime.Serialization;

namespace WorldRank.Console
{
	public class InMemoryPlayerRepository : IPlayerRepository
	{
		private List<Player> _players;

		public InMemoryPlayerRepository(List<Player> players)
		{
			_players = players;
		}

		public void AddPlayer(Player player)
		{
			_players.Add(player);
		}

		public void DeletePlayer(int playerId)
		{
			var player = _players.Where(p => p.Id == playerId).FirstOrDefault();

			if (player != null)
			{
				_players.Remove(player);
			}
		}

		public Player? FindPlayer(int playerId)
		{
			return _players.Where(item => item.Id == playerId).FirstOrDefault();
		}

		public IEnumerable<IGrouping<int, Player>> GroupPlayersByScore()
		{
            // Group players by score and then order the groups by score in ascending order
            return _players.OrderBy(p => p.Score).GroupBy(p => p.Score).OrderBy(g => g.Key);
        }
	}
}