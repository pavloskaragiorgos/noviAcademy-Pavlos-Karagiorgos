using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WorldRank.Application.Interfaces;
using WorldRank.Domain.Entities;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;
using WorldRank.Infrastructure.Persistence;

namespace WorldRank.Infrastructure.Repositories;

public class DBWalletRepository : IWalletRepository
{
    private readonly WorldRankDbContext _context;
    private readonly ILogger<DBWalletRepository> _logger;

    public DBWalletRepository(WorldRankDbContext context, ILogger<DBWalletRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(Wallet wallet, CancellationToken cancellationToken = default)
    {
        var exists = await _context.Wallets.AnyAsync(item => item.PlayerId == wallet.PlayerId && item.Currency == wallet.Currency, cancellationToken);

        if (exists)
        {
            throw new DuplicateWalletException(wallet.PlayerId, wallet.Currency);
        }

        await _context.Wallets.AddAsync(wallet, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Wallet created for player {PlayerId} in {Currency} with balance {Balance}", wallet.PlayerId, wallet.Currency, wallet.Balance);
    }

    public async Task<Wallet[]> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Wallets.AsNoTracking().ToArrayAsync(cancellationToken);
    }

    public async Task<List<Wallet>> GetAllWalletsByPlayerIdAsync(int playerId, CancellationToken cancellationToken = default)
    {
        return await _context.Wallets.AsNoTracking().Where(item => item.PlayerId == playerId).ToListAsync(cancellationToken);
    }

    public async Task<Wallet?> GetWalletByIdAsync(int walletId, CancellationToken cancellationToken = default)
    {
        return await _context.Wallets.AsNoTracking().FirstOrDefaultAsync(item => item.Id == walletId, cancellationToken);
    }

    public async Task UpdateBalanceAsync(int playerId, Currency currency, decimal newBalance, CancellationToken cancellationToken = default)
    {
        var wallet = await GetTrackedWalletAsync(playerId, currency, cancellationToken);
        wallet.SetBalance(newBalance);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Player {PlayerId} {Currency} wallet balance set to {Balance}", playerId, currency, newBalance);
    }

    public async Task DepositAsync(int playerId, Currency currency, decimal amount, CancellationToken cancellationToken = default)
    {
        var wallet = await GetTrackedWalletAsync(playerId, currency, cancellationToken);
        wallet.Deposit(amount);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Deposited {Amount} to player {PlayerId} {Currency} wallet (balance {Balance})", amount, playerId, currency, wallet.Balance);
    }

    public async Task WithdrawAsync(int playerId, Currency currency, decimal amount, CancellationToken cancellationToken = default)
    {
        var wallet = await GetTrackedWalletAsync(playerId, currency, cancellationToken);
        wallet.Withdraw(amount);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Withdrew {Amount} from player {PlayerId} {Currency} wallet (balance {Balance})", amount, playerId, currency, wallet.Balance);
    }

    public async Task BlockAsync(int playerId, Currency currency, CancellationToken cancellationToken = default)
    {
        var wallet = await GetTrackedWalletAsync(playerId, currency, cancellationToken);
        wallet.Block();
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Player {PlayerId} {Currency} wallet blocked", playerId, currency);
    }

    public async Task UnblockAsync(int playerId, Currency currency, CancellationToken cancellationToken = default)
    {
        var wallet = await GetTrackedWalletAsync(playerId, currency, cancellationToken);
        wallet.Unblock();
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Player {PlayerId} {Currency} wallet unblocked", playerId, currency);
    }

    public async Task<Wallet> GetWalletAsync(int playerId, Currency currency, CancellationToken cancellationToken = default)
    {
        return await GetTrackedWalletAsync(playerId, currency, cancellationToken);
    }

    private async Task<Wallet> GetTrackedWalletAsync(int playerId, Currency currency, CancellationToken cancellationToken)
    {
        var wallet = await _context.Wallets.SingleOrDefaultAsync(item => item.PlayerId == playerId && item.Currency == currency, cancellationToken);

        if (wallet is null)
        {
            throw new WalletNotFoundException(playerId, currency);
        }

        return wallet;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
}