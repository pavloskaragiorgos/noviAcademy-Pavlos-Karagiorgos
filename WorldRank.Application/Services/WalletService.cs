using WorldRank.Application.Interfaces;
using WorldRank.Domain.Entities;
using WorldRank.Domain.Enums;

namespace WorldRank.Application.Services;

public class WalletService
{
    private readonly IWalletRepository _walletRepository;

    public WalletService(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public void AddWallet(Wallet wallet)
    {
        _walletRepository.Add(wallet);
    }

    public List<Wallet> GetWalletsByPlayerId(int playerId)
    {
        return _walletRepository.GetAllWalletsByPlayerId(playerId);
    }

    public void Deposit(int playerId, Currency currency, decimal amount)
    {
        _walletRepository.Deposit(playerId, currency, amount);
    }

    public void Withdraw(int playerId, Currency currency, decimal amount)
    {
        _walletRepository.Withdraw(playerId, currency, amount);
    }

    public void BlockWallet(int playerId, Currency currency)
    {
        _walletRepository.Block(playerId, currency);
    }

    public void UnblockWallet(int playerId, Currency currency)
    {
        _walletRepository.Unblock(playerId, currency);
    }

    public void UpdateBalance(int playerId, Currency currency, decimal newBalance)
    {
        _walletRepository.UpdateBalance(playerId, currency, newBalance);
    }
}
