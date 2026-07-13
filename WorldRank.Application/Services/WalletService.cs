using Microsoft.Extensions.Logging;
using WorldRank.Application.Interfaces;
using WorldRank.Application.Strategies;
using WorldRank.Domain.Entities;
using WorldRank.Domain.Enums;
using WorldRank.Domain.Exceptions;

namespace WorldRank.Application.Services;

public class WalletService
{
    private readonly IWalletRepository _walletRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ILogger<WalletService> _logger;
    private readonly IReadOnlyDictionary<FundsOperation, IFundsStrategy> _fundsStrategies;

    public WalletService(
        IWalletRepository walletRepository,
        IPlayerRepository playerRepository,
        IEnumerable<IFundsStrategy> strategies,
        ILogger<WalletService> logger)
    {
        _walletRepository = walletRepository;
        _playerRepository = playerRepository;
        _logger = logger;

        // Index every registered strategy by the operation it implements.
        _fundsStrategies = strategies.ToDictionary(strategy => strategy.Operation);
    }

    public Wallet AddWalletToPlayer(int playerId, Currency currency, decimal balance)
    {
        if (_playerRepository.FindPlayer(playerId) is null)
            throw new PlayerNotFoundException(playerId);

        var wallet = new Wallet(GenerateWalletId(), playerId, currency, balance);
        _walletRepository.Add(wallet);
        return wallet;
    }

    public Wallet? GetWalletById(int walletId)
    {
        return _walletRepository.GetWalletById(walletId);
    }

    public List<Wallet> GetWalletsOfPlayer(int playerId)
    {
        return _walletRepository.GetAllWalletsByPlayerId(playerId);
    }

    public void DepositToWallet(int playerId, Currency currency, decimal amount)
    {
        RunWalletOperation(() =>
        {
            _walletRepository.Deposit(playerId, currency, amount);
        });
    }

    public void WithdrawFromWallet(int playerId, Currency currency, decimal amount)
    {
        RunWalletOperation(() =>
        {
            _walletRepository.Withdraw(playerId, currency, amount);
        });
    }

    public void BlockWallet(int playerId, Currency currency)
    {
        RunWalletOperation(() =>
        {
            _walletRepository.Block(playerId, currency);
        });
    }

    public void UnblockWallet(int playerId, Currency currency)
    {
        RunWalletOperation(() =>
        {
            _walletRepository.Unblock(playerId, currency);
        });
    }

    public void UpdateWalletBalance(int playerId, Currency currency, decimal newBalance)
    {
        RunWalletOperation(() =>
        {
            _walletRepository.UpdateBalance(playerId, currency, newBalance);
        });
    }

    public void ApplyFundsStrategy(int playerId, Currency currency, FundsOperation operation ,decimal amount)
    {
        // Pick the strategy that matches the chosen operation (resolved from DI, no factory).
        var strategy = _fundsStrategies[operation];

        RunWalletOperation(() =>
        {
            var wallet = _walletRepository.GetWallet(playerId, currency);
            strategy.Execute(wallet, amount);
            _logger.LogInformation("Applied {Strategy} of {Amount} to player {PlayerId} {Currency} wallet (balance {Balance})",
                strategy.GetType().Name, amount, playerId, currency, wallet.Balance);
        });
    }

    // Runs a wallet operation, logs any domain (WalletException) failure, then lets it propagate to the caller.
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

