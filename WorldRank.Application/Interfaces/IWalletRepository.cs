using WorldRank.Domain.Enums;
using WorldRank.Domain.Entities;

namespace WorldRank.Application.Interfaces
{
	public interface IWalletRepository
	{
		void Add(Wallet wallet);
		Wallet[] GetAll();
		Wallet GetWallet(int playerId, Currency currency);
        Wallet? GetWalletById(int walletId);
        List<Wallet> GetAllWalletsByPlayerId(int playerId);

		void UpdateBalance(int playerId, Currency currency, decimal newBalance);

		void Deposit(int playerId, Currency currency, decimal amount);

		void Withdraw(int playerId, Currency currency, decimal amount);

		void Block(int playerId, Currency currency);

		void Unblock(int playerId, Currency currency);

        // Commit pending changes (Unit of Work). Used by WalletService.ApplyFundsStrategy,
        // which mutates a wallet via a strategy and needs an explicit save.
        void SaveChanges();
    }
}
