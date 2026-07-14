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

    public Wallet AddWalletToPlayer(int playerId, Currency currency, decimal balance)
    {
        if (_playerRepository.FindPlayer(playerId) is null)
            throw new PlayerNotFoundException(playerId);

        var wallet = new Wallet(GenerateWalletId(), playerId, currency, balance);
        _walletRepository.Add(wallet);
        _logger.LogInformation("Wallet created {WalletId} for player {PlayerId} in {Currency}", wallet.Id, playerId, currency);
        Refresh(wallet);
        return wallet;
    }

    public Wallet? GetWalletById(int walletId)
    {
        if (_cache.TryGet(WalletKey(walletId), out Wallet? cached) && cached is not null)
        {
            _logger.LogInformation("Cache HIT  wallet {WalletId}", walletId);
            return cached;
        }

        _logger.LogInformation("Cache MISS wallet {WalletId} — loading from database", walletId);
        var wallet = _walletRepository.GetWalletById(walletId);
        if (wallet is not null)
            _cache.Set(WalletKey(walletId), wallet, Ttl);
        return wallet;
    }

    public List<Wallet> GetWalletsOfPlayer(int playerId)
    {
        var key = PlayerWalletsKey(playerId);

        if (_cache.TryGet(key, out List<Wallet>? cached) && cached is not null)
        {
            _logger.LogInformation("Cache HIT  wallets of player {PlayerId}", playerId);
            return cached;
        }

        _logger.LogInformation("Cache MISS wallets of player {PlayerId} — loading from database", playerId);
        var wallets = _walletRepository.GetAllWalletsByPlayerId(playerId);
        _cache.Set(key, wallets, Ttl);
        return wallets;
    }

    public void DepositToWallet(int playerId, Currency currency, decimal amount)
    {
        RunWalletOperation(() =>
        {
            _walletRepository.Deposit(playerId, currency, amount);
            Refresh(_walletRepository.GetWallet(playerId, currency));
        });
    }

    public void WithdrawFromWallet(int playerId, Currency currency, decimal amount)
    {
        RunWalletOperation(() =>
        {
            _walletRepository.Withdraw(playerId, currency, amount);
            Refresh(_walletRepository.GetWallet(playerId, currency));
        });
    }

    public void BlockWallet(int playerId, Currency currency)
    {
        RunWalletOperation(() =>
        {
            _walletRepository.Block(playerId, currency);
            Refresh(_walletRepository.GetWallet(playerId, currency));
        });
    }

    public void UnblockWallet(int playerId, Currency currency)
    {
        RunWalletOperation(() =>
        {
            _walletRepository.Unblock(playerId, currency);
            Refresh(_walletRepository.GetWallet(playerId, currency));
        });
    }

    public void UpdateWalletBalance(int playerId, Currency currency, decimal newBalance)
    {
        RunWalletOperation(() =>
        {
            _walletRepository.UpdateBalance(playerId, currency, newBalance);
            Refresh(_walletRepository.GetWallet(playerId, currency));
        });
    }

    public void ApplyFundsStrategy(int playerId, Currency currency, FundsOperation operation, decimal amount)
    {
        var strategy = _fundsStrategies[operation];

        RunWalletOperation(() =>
        {
            var wallet = _walletRepository.GetWallet(playerId, currency);
            strategy.Execute(wallet, amount);
            _walletRepository.SaveChanges();
            _logger.LogInformation("Applied {Strategy} of {Amount} to player {PlayerId} {Currency} wallet (balance {Balance})",
                strategy.GetType().Name, amount, playerId, currency, wallet.Balance);
            Refresh(wallet);
        });
    }

    // Write-through: store the fresh wallet under its own key and drop the player's wallet-list
    // cache, so the next read rebuilds it from the database.
    private void Refresh(Wallet wallet)
    {
        _cache.Set(WalletKey(wallet.Id), wallet, Ttl);
        _cache.Remove(PlayerWalletsKey(wallet.PlayerId));
        _logger.LogInformation("Cache write-through wallet {WalletId}; list cache invalidated", wallet.Id);
    }

    private void RunWalletOperation(Action operation)
    {
        try
        {
            operation();
        }
        catch (WalletException ex)
        {
            _logger.LogWarning(ex, "Wallet operation failed");
            throw;
        }
    }

    private int GenerateWalletId()
    {
        var existingIds = _walletRepository.GetAll().Select(p => p.Id).ToHashSet();

        int id;
        do
        {
            id = Random.Shared.Next(1, int.MaxValue);
        }
        while (existingIds.Contains(id));

        return id;
    }
}