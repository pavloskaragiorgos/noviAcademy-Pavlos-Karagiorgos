using System.ComponentModel;

interface IWalletRepository
{
    public void Add(Wallet wallet, Guid playerId);
    public List<Wallet> GetByPlayer(Guid playerId); 

}