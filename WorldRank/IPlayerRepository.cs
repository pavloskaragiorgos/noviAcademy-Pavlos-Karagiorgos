using System;
using System.Collections.Generic;
using System.Text;

namespace WorldRank
{
    interface IPlayerRepository
    {
        public void AddPLayer(Player player);
        public void DeletePLayer(Player player);
        public Player FindPlayer(Guid id);

    }
}
