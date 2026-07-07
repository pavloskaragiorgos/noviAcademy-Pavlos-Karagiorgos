using System.ComponentModel;

interface IWalletRepository
{
    public void Add(Wallet wallet, int playerId);
    public List<Wallet> GetByPlayer(int playerId); 

}