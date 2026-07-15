using WorldRank.Domain.Enums;
using WorldRank.Domain.Entities;

namespace WorldRank.Application.Interfaces;

public interface IWalletRepository
{
    Task AddAsync(Wallet wallet, CancellationToken cancellationToken = default);
    Task<Wallet[]> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Wallet> GetWalletAsync(int playerId, Currency currency, CancellationToken cancellationToken = default);
    Task<Wallet?> GetWalletByIdAsync(int walletId, CancellationToken cancellationToken = default);
    Task<List<Wallet>> GetAllWalletsByPlayerIdAsync(int playerId, CancellationToken cancellationToken = default);

    Task UpdateBalanceAsync(int playerId, Currency currency, decimal newBalance, CancellationToken cancellationToken = default);
    Task DepositAsync(int playerId, Currency currency, decimal amount, CancellationToken cancellationToken = default);
    Task WithdrawAsync(int playerId, Currency currency, decimal amount, CancellationToken cancellationToken = default);
    Task BlockAsync(int playerId, Currency currency, CancellationToken cancellationToken = default);
    Task UnblockAsync(int playerId, Currency currency, CancellationToken cancellationToken = default);

    // Commit pending changes (Unit of Work). Used by WalletService.ApplyFundsStrategyAsync,
    // which mutates a wallet via a strategy and needs an explicit save.
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}