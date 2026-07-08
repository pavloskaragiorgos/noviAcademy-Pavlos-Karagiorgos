using WorldRank;

public class InMemoryWalletRepository : IWalletRepository
{
	private List<Player> _players;
	public InMemoryWalletRepository(List<Player> players)
	{
		_players = players;
	}
	public void Add(Wallet wallet, Guid playerId)
	{
		var player = _players.Where(p => p.Id == playerId).FirstOrDefault();
		if (player != null) 
			player.Wallets.Add(wallet.Currency, wallet);
    }
	public List<Wallet> GetByPlayer(Guid playerId) 
	{
        var wallets = _players.Where(p => p.Id == playerId).SelectMany(p => p.Wallets.Values);
        return wallets.ToList();
    }
}