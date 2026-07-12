using Microsoft.Extensions.DependencyInjection;
using NLog;
using WorldRank.Console;
using WorldRank.Console.Services;

var logger = LogManager.GetCurrentClassLogger();

var services = new ServiceCollection();
services.AddWorldRank();

using var provider = services.BuildServiceProvider();

var playerMenu = provider.GetRequiredService<PlayerMenu>();
var walletMenu = provider.GetRequiredService<WalletMenu>();

logger.Info("Application started.");

while (true)
{
	Console.WriteLine("\n=== WorldRank Player Registry ===");
	Console.WriteLine("--- Players ---");
	Console.WriteLine("1. Add player");
	Console.WriteLine("2. List all players");
	Console.WriteLine("3. List players grouped by score");
	Console.WriteLine("4. Find player by name");
	Console.WriteLine("5. Find player by id");
	Console.WriteLine("6. Delete player");
	Console.WriteLine("--- Wallets ---");
	Console.WriteLine("7. Add wallet to player");
	Console.WriteLine("8. Show player wallets");
	Console.WriteLine("9. Deposit to wallet");
	Console.WriteLine("10. Withdraw from wallet");
	Console.WriteLine("11. Block wallet");
	Console.WriteLine("12. Unblock wallet");
	Console.WriteLine("13. Update wallet balance");
	Console.WriteLine("14. Apply funds operation");
	Console.WriteLine("0. Exit");
	Console.Write("> ");

    Action? action = Console.ReadLine() switch
    {
        "1" => playerMenu.AddPlayer,
        "2" => playerMenu.ListPlayers,
        "3" => playerMenu.ListPlayersByScore,
        "4" => playerMenu.FindPlayerByName,
        "5" => playerMenu.FindPlayerById,
        "6" => playerMenu.DeletePlayer,
        "7" => walletMenu.AddWalletToPlayer,
        "8" => walletMenu.GetWalletsOfPlayer,
        "9" => walletMenu.DepositToWallet,
        "10" => walletMenu.WithdrawFromWallet,
        "11" => walletMenu.BlockWallet,
        "12" => walletMenu.UnblockWallet,
        "13" => walletMenu.UpdateWalletBalance,
        "14" => walletMenu.ApplyFundsStrategy,
        "0" => null,
        _ => () => Console.WriteLine("Unknown option.")
    };

    if (action is null)
	{
		logger.Info("Application exiting.");
		LogManager.Shutdown(); // flush file writes before exit
		return; // "0" selected — exit
	}

	try
	{
		action();
	}
	catch (Exception ex)
	{
		// Safety net: log any exception the specific handlers did not catch, and keep the app running.
		logger.Error(ex, "Unexpected error while handling a menu action");
		Console.WriteLine($"Unexpected error: {ex.Message}");
	}
}



