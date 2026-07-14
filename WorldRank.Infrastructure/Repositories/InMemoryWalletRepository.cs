using Microsoft.Extensions.Logging;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Entities;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;

namespace WorldRank.Infrastructure.Repositories
{
    public class InMemoryWalletRepository : IWalletRepository
    {
        private readonly ILogger<InMemoryWalletRepository> _logger;

        private readonly List<Wallet> _wallets = new List<Wallet>();

        public InMemoryWalletRepository(ILogger<InMemoryWalletRepository> logger)
        {
            _logger = logger;
        }

        public Task AddAsync(Wallet wallet, CancellationToken cancellationToken = default)
        {
            var exists = _wallets.Any(item => item.PlayerId == wallet.PlayerId && item.Currency == wallet.Currency);

            if (exists)
            {
                throw new DuplicateWalletException(wallet.PlayerId, wallet.Currency);
            }

            _wallets.Add(wallet);
            _logger.LogInformation("Wallet created for player {PlayerId} in {Currency} with balance {Balance}", wallet.PlayerId, wallet.Currency, wallet.Balance);
            return Task.CompletedTask;
        }

        public Task<Wallet[]> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_wallets.ToArray());
        }

        public Task<List<Wallet>> GetAllWalletsByPlayerIdAsync(int playerId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_wallets.Where(item => item.PlayerId == playerId).ToList());
        }

        public Task<Wallet?> GetWalletByIdAsync(int walletId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_wallets.FirstOrDefault(item => item.Id == walletId));
        }

        public Task UpdateBalanceAsync(int playerId, Currency currency, decimal newBalance, CancellationToken cancellationToken = default)
        {
            GetTrackedWallet(playerId, currency).SetBalance(newBalance);
            _logger.LogInformation("Player {PlayerId} {Currency} wallet balance set to {Balance}", playerId, currency, newBalance);
            return Task.CompletedTask;
        }

        public Task DepositAsync(int playerId, Currency currency, decimal amount, CancellationToken cancellationToken = default)
        {
            var wallet = GetTrackedWallet(playerId, currency);
            wallet.Deposit(amount);
            _logger.LogInformation("Deposited {Amount} to player {PlayerId} {Currency} wallet (balance {Balance})", amount, playerId, currency, wallet.Balance);
            return Task.CompletedTask;
        }

        public Task WithdrawAsync(int playerId, Currency currency, decimal amount, CancellationToken cancellationToken = default)
        {
            var wallet = GetTrackedWallet(playerId, currency);
            wallet.Withdraw(amount);
            _logger.LogInformation("Withdrew {Amount} from player {PlayerId} {Currency} wallet (balance {Balance})", amount, playerId, currency, wallet.Balance);
            return Task.CompletedTask;
        }

        public Task BlockAsync(int playerId, Currency currency, CancellationToken cancellationToken = default)
        {
            GetTrackedWallet(playerId, currency).Block();
            _logger.LogInformation("Player {PlayerId} {Currency} wallet blocked", playerId, currency);
            return Task.CompletedTask;
        }

        public Task UnblockAsync(int playerId, Currency currency, CancellationToken cancellationToken = default)
        {
            GetTrackedWallet(playerId, currency).Unblock();
            _logger.LogInformation("Player {PlayerId} {Currency} wallet unblocked", playerId, currency);
            return Task.CompletedTask;
        }

        public Task<Wallet> GetWalletAsync(int playerId, Currency currency, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GetTrackedWallet(playerId, currency));
        }

        private Wallet GetTrackedWallet(int playerId, Currency currency)
        {
            var wallet = _wallets.SingleOrDefault(item => item.PlayerId == playerId && item.Currency == currency);

            if (wallet is null)
            {
                throw new WalletNotFoundException(playerId, currency);
            }

            return wallet;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}