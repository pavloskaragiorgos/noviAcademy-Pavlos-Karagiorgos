using System;
using System.Collections.Generic;
using System.Text;

namespace WorldRank
{
    public class InMemoryPlayerRepository : IPlayerRepository
    {
        private List<Player> _players;
        public InMemoryPlayerRepository(List<Player> players)
        {
            _players = players ;
        }
        public void AddPLayer(Player player)
        {
            _players.Add(player);
        }
        public void DeletePLayer(Guid playerId)
        {
            var playerToDelete = _players.Where(item => item.Id == playerId).FirstOrDefault();
            if (playerToDelete != null) 
                _players.Remove(playerToDelete);
        }
        public Player FindPlayer(Guid playerId)
        {
            return _players.Where(item => item.Id == playerId).FirstOrDefault();
        }


        
    }
}
