using System;
using System.Collections.Generic;
using System.Text;

namespace WorldRank
{
    public class InMemoryPlayerRepository : IPlayerRepository
    {
        Dictionary<Guid,Player> _players;
        public InMemoryPlayerRepository()
        {
            _players = new Dictionary<Guid, Player>();
        }
        public void AddPLayer(Player player)
        {
            _players.Add(player.Id, player);
        }
        public void DeletePLayer(Player player)
        {
            _players.Remove(player.Id);
        }
        public Player FindPlayer(Guid id)
        {
            if(_players.Count == 0) 
            {
                throw new Exception("No players found in the repository.");
            }
            return _players[id];
        }

        
    }
}
