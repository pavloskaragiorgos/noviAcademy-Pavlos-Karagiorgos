using System;
using WorldRank.Domain.Exceptions;
using AppWalletService = WorldRank.Application.Services.WalletService;

namespace WorldRank.Console.Services
{
    internal class WalletMenu
    {
        private readonly AppWalletService _walletService;

        public WalletMenu(AppWalletService walletService)
        {
            _walletService = walletService;
        }

        public void AddWalletToPlayer()
        {
            var playerId = Prompts.PromptPlayerId();
            if (playerId is null)
                return;

            var currency = Prompts.PromptCurrency();
            if (currency is null)
                return;

            var balance = Prompts.PromptAmount("Initial balance");
            if (balance is null)
                return;

            try
            {
                _walletService.AddWalletToPlayer(playerId.Value, currency.Value, balance.Value);
                System.Console.WriteLine("Wallet added successfully.");
            }
            catch (PlayerNotFoundException ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
            }
            catch (WalletException ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void GetWalletsOfPlayer()
        {
            var playerId = Prompts.PromptPlayerId();
            if (playerId is null)
                return;

            var wallets = _walletService.GetWalletsOfPlayer(playerId.Value);

            if (wallets.Count == 0)
            {
                System.Console.WriteLine("No wallets found for this player.");
                return;
            }

            foreach (var wallet in wallets)
                System.Console.WriteLine($"Wallet Number {wallets.IndexOf(wallet)} {wallet}");
        }

        public void DepositToWallet()
        {
            var playerId = Prompts.PromptPlayerId();
            if (playerId is null)
                return;

            var currency = Prompts.PromptCurrency();
            if (currency is null)
                return;

            var amount = Prompts.PromptAmount("Amount to deposit");
            if (amount is null)
                return;

            try
            {
                _walletService.DepositToWallet(playerId.Value, currency.Value, amount.Value);
                System.Console.WriteLine("Deposit successful.");
            }
            catch (WalletException ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void WithdrawFromWallet()
        {
            var playerId = Prompts.PromptPlayerId();
            if (playerId is null)
                return;

            var currency = Prompts.PromptCurrency();
            if (currency is null)
                return;

            var amount = Prompts.PromptAmount("Amount to withdraw");
            if (amount is null)
                return;

            try
            {
                _walletService.WithdrawFromWallet(playerId.Value, currency.Value, amount.Value);
                System.Console.WriteLine("Withdrawal successful.");
            }
            catch (WalletException ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void BlockWallet()
        {
            var playerId = Prompts.PromptPlayerId();
            if (playerId is null)
                return;

            var currency = Prompts.PromptCurrency();
            if (currency is null)
                return;

            try
            {
                _walletService.BlockWallet(playerId.Value, currency.Value);
                System.Console.WriteLine("Wallet blocked.");
            }
            catch (WalletException ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void UnblockWallet()
        {
            var playerId = Prompts.PromptPlayerId();
            if (playerId is null)
                return;

            var currency = Prompts.PromptCurrency();
            if (currency is null)
                return;

            try
            {
                _walletService.UnblockWallet(playerId.Value, currency.Value);
                System.Console.WriteLine("Wallet unblocked.");
            }
            catch (WalletException ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void UpdateWalletBalance()
        {
            var playerId = Prompts.PromptPlayerId();
            if (playerId is null)
                return;

            var currency = Prompts.PromptCurrency();
            if (currency is null)
                return;

            var newBalance = Prompts.PromptAmount("New balance");
            if (newBalance is null)
                return;

            try
            {
                _walletService.UpdateWalletBalance(playerId.Value, currency.Value, newBalance.Value);
                System.Console.WriteLine("Balance updated.");
            }
            catch (WalletException ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void ApplyFundsStrategy()
        {
            var playerId = Prompts.PromptPlayerId();
            if (playerId is null)
                return;

            var currency = Prompts.PromptCurrency();
            if (currency is null)
                return;

            var operation = Prompts.PromptFundsOperation();
            if (operation is null)
                return;

            var amount = Prompts.PromptAmount("Amount");
            if (amount is null)
                return;

            try
            {
                _walletService.ApplyFundsStrategy(playerId.Value, currency.Value, operation.Value, amount.Value);
                System.Console.WriteLine($"{operation} operation applied.");
            }
            catch (WalletException ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}");
            }
        }




    }
}
