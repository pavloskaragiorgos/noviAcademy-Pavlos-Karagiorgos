using WorldRank;

public class InMemoryWalletRepository : IWalletRepository
{
	Dictionary<Guid,Player> _players;
	public InMemoryWalletRepository(Dictionary<Guid, Player> players)
	{
		_players = players;
	}
	public void Add(Wallet wallet, Guid playerId)
	{
		Player p = _players[playerId];
		p.AddWallet(wallet.Currency 
	}
	public List<Wallet> GetByPlayer(int playerId) 
	{ 
		return new List<Wallet>();
	}
}