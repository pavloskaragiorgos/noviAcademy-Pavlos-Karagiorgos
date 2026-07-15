using Microsoft.Extensions.Logging;
using WorldRank.Application.Interfaces;
using WorldRank.Application.Strategies;
using WorldRank.Domain.Entities;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;

namespace WorldRank.Application.Services;

public class WalletService
{
    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(60);

    private readonly IWalletRepository _walletRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ICache _cache;
    private readonly ILogger<WalletService> _logger;
    private readonly IReadOnlyDictionary<FundsOperation, IFundsStrategy> _fundsStrategies;

    public WalletService(
        IWalletRepository walletRepository,
        IPlayerRepository playerRepository,
        IEnumerable<IFundsStrategy> strategies,
        ICache cache,
        ILogger<WalletService> logger)
    {
        _walletRepository = walletRepository;
        _playerRepository = playerRepository;
        _cache = cache;
        _logger = logger;
        _fundsStrategies = strategies.ToDictionary(strategy => strategy.Operation);
    }

    private static string WalletKey(int walletId) => $"wallet:{walletId}";
    private static string PlayerWalletsKey(int playerId) => $"wallets:player:{playerId}";

    public async Task<Wallet> AddWalletToPlayerAsync(int playerId, Currency currency, decimal balance, CancellationToken cancellationToken = default)
    {
        if (await _playerRepository.FindPlayerAsync(playerId, cancellationToken) is null)
            throw new PlayerNotFoundException(playerId);

        var wallet = new Wallet(await GenerateWalletIdAsync(cancellationToken), playerId, currency, balance);
        await _walletRepository.AddAsync(wallet, cancellationToken);
        _logger.LogInformation("Wallet created {WalletId} for player {PlayerId} in {Currency}", wallet.Id, playerId, currency);
        Refresh(wallet);
        return wallet;
    }

    public async Task<Wallet?> GetWalletByIdAsync(int walletId, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGet(WalletKey(walletId), out Wallet? cached) && cached is not null)
        {
            _logger.LogInformation("Cache HIT  wallet {WalletId}", walletId);
            return cached;
        }

        _logger.LogInformation("Cache MISS wallet {WalletId} — loading from database", walletId);
        var wallet = await _walletRepository.GetWalletByIdAsync(walletId, cancellationToken);
        if (wallet is not null)
            _cache.Set(WalletKey(walletId), wallet, Ttl);
        return wallet;
    }

    public async Task<List<Wallet>> GetWalletsOfPlayerAsync(int playerId, CancellationToken cancellationToken = default)
    {
        var key = PlayerWalletsKey(playerId);

        if (_cache.TryGet(key, out List<Wallet>? cached) && cached is not null)
        {
            _logger.LogInformation("Cache HIT  wallets of player {PlayerId}", playerId);
            return cached;
        }

        _logger.LogInformation("Cache MISS wallets of player {PlayerId} — loading from database", playerId);
        var wallets = await _walletRepository.GetAllWalletsByPlayerIdAsync(playerId, cancellationToken);
        _cache.Set(key, wallets, Ttl);
        return wallets;
    }

    public async Task DepositToWalletAsync(int playerId, Currency currency, decimal amount, CancellationToken cancellationToken = default)
    {
        await RunWalletOperationAsync(async () =>
        {
            await _walletRepository.DepositAsync(playerId, currency, amount, cancellationToken);
            Refresh(await _walletRepository.GetWalletAsync(playerId, currency, cancellationToken));
        });
    }

    public async Task WithdrawFromWalletAsync(int playerId, Currency currency, decimal amount, CancellationToken cancellationToken = default)
    {
        await RunWalletOperationAsync(async () =>
        {
            await _walletRepository.WithdrawAsync(playerId, currency, amount, cancellationToken);
            Refresh(await _walletRepository.GetWalletAsync(playerId, currency, cancellationToken));
        });
    }

    public async Task BlockWalletAsync(int playerId, Currency currency, CancellationToken cancellationToken = default)
    {
        await RunWalletOperationAsync(async () =>
        {
            await _walletRepository.BlockAsync(playerId, currency, cancellationToken);
            Refresh(await _walletRepository.GetWalletAsync(playerId, currency, cancellationToken));
        });
    }

    public async Task UnblockWalletAsync(int playerId, Currency currency, CancellationToken cancellationToken = default)
    {
        await RunWalletOperationAsync(async () =>
        {
            await _walletRepository.UnblockAsync(playerId, currency, cancellationToken);
            Refresh(await _walletRepository.GetWalletAsync(playerId, currency, cancellationToken));
        });
    }

    public async Task UpdateWalletBalanceAsync(int playerId, Currency currency, decimal newBalance, CancellationToken cancellationToken = default)
    {
        await RunWalletOperationAsync(async () =>
        {
            await _walletRepository.UpdateBalanceAsync(playerId, currency, newBalance, cancellationToken);
            Refresh(await _walletRepository.GetWalletAsync(playerId, currency, cancellationToken));
        });
    }

    public async Task ApplyFundsStrategyAsync(int playerId, Currency currency, FundsOperation operation, decimal amount, CancellationToken cancellationToken = default)
    {
        var strategy = _fundsStrategies[operation];

        await RunWalletOperationAsync(async () =>
        {
            var wallet = await _walletRepository.GetWalletAsync(playerId, currency, cancellationToken);
            strategy.Execute(wallet, amount);
            await _walletRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Applied {Strategy} of {Amount} to player {PlayerId} {Currency} wallet (balance {Balance})",
                strategy.GetType().Name, amount, playerId, currency, wallet.Balance);
            Refresh(wallet);
        });
    }

    private void Refresh(Wallet wallet)
    {
        _cache.Set(WalletKey(wallet.Id), wallet, Ttl);
        _cache.Remove(PlayerWalletsKey(wallet.PlayerId));
        _logger.LogInformation("Cache write-through wallet {WalletId}; list cache invalidated", wallet.Id);
    }

    private async Task RunWalletOperationAsync(Func<Task> operation)
    {
        try
        {
            await operation();
        }
        catch (WalletException ex)
        {
            _logger.LogWarning(ex, "Wallet operation failed");
            throw;
        }
    }

    private async Task<int> GenerateWalletIdAsync(CancellationToken cancellationToken)
    {
        var existingIds = (await _walletRepository.GetAllAsync(cancellationToken)).Select(p => p.Id).ToHashSet();

        int id;
        do
        {
            id = Random.Shared.Next(1, int.MaxValue);
        }
        while (existingIds.Contains(id));

        return id;
    }
}