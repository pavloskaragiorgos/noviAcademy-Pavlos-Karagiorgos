using Microsoft.AspNetCore.Mvc;
using WorldRank.Api.Dtos;
using WorldRank.Domain.Entities;
using WorldRank.Domain.Exceptions;
using AppWalletService = WorldRank.Application.Services.WalletService;

namespace WorldRank.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalletsController : ControllerBase
    {
        private readonly AppWalletService _walletService;
        private readonly ILogger<WalletsController> _logger;

        public WalletsController(AppWalletService walletService, ILogger<WalletsController> logger)
        {
            _walletService = walletService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var wallet = await _walletService.AddWalletToPlayerAsync(request.PlayerId, request.Currency, request.Balance, cancellationToken);
                return CreatedAtAction(nameof(GetWalletById), new { id = wallet.Id }, wallet);
            }
            catch (PlayerNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (WalletException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating wallet");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetWalletById(int id, CancellationToken cancellationToken)
        {
            var wallet = await _walletService.GetWalletByIdAsync(id, cancellationToken);
            if (wallet is null) return NotFound();

            return Ok(wallet);
        }

        [HttpGet("player/{playerId:int}")]
        public async Task<IActionResult> GetWalletsOfPlayer(int playerId, CancellationToken cancellationToken)
        {
            try
            {
                var wallets = await _walletService.GetWalletsOfPlayerAsync(playerId, cancellationToken);
                return Ok(wallets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching wallets for player {PlayerId}", playerId);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost("{id:int}/deposit")]
        public Task<IActionResult> Deposit(int id, [FromBody] DepositRequest request, CancellationToken cancellationToken) =>
            MutateWalletAsync(id, wallet => _walletService.DepositToWalletAsync(wallet.PlayerId, wallet.Currency, request.Amount, cancellationToken), cancellationToken);

        [HttpPost("{id:int}/withdraw")]
        public Task<IActionResult> Withdraw(int id, [FromBody] WithdrawRequest request, CancellationToken cancellationToken) =>
            MutateWalletAsync(id, wallet => _walletService.WithdrawFromWalletAsync(wallet.PlayerId, wallet.Currency, request.Amount, cancellationToken), cancellationToken);

        [HttpPost("{id:int}/block")]
        public Task<IActionResult> Block(int id, CancellationToken cancellationToken) =>
            MutateWalletAsync(id, wallet => _walletService.BlockWalletAsync(wallet.PlayerId, wallet.Currency, cancellationToken), cancellationToken);

        [HttpPost("{id:int}/unblock")]
        public Task<IActionResult> Unblock(int id, CancellationToken cancellationToken) =>
            MutateWalletAsync(id, wallet => _walletService.UnblockWalletAsync(wallet.PlayerId, wallet.Currency, cancellationToken), cancellationToken);

        [HttpPut("{id:int}/balance")]
        public Task<IActionResult> UpdateBalance(int id, [FromBody] UpdateBalanceRequest request, CancellationToken cancellationToken) =>
            MutateWalletAsync(id, wallet => _walletService.UpdateWalletBalanceAsync(wallet.PlayerId, wallet.Currency, request.NewBalance, cancellationToken), cancellationToken);

        [HttpPost("{id:int}/fund-operation")]
        public Task<IActionResult> ApplyFundsOperation(int id, [FromBody] ApplyFundsOperationRequest request, CancellationToken cancellationToken) =>
            MutateWalletAsync(id, wallet => _walletService.ApplyFundsStrategyAsync(wallet.PlayerId, wallet.Currency, request.Operation, request.Amount, cancellationToken), cancellationToken);

        // Shared shape for every "mutate an existing wallet, then return the fresh state" endpoint:
        // 404 if the wallet doesn't exist, 400 for domain rule violations, 500 for anything else.
        private async Task<IActionResult> MutateWalletAsync(int id, Func<Wallet, Task> mutate, CancellationToken cancellationToken)
        {
            var wallet = await _walletService.GetWalletByIdAsync(id, cancellationToken);
            if (wallet is null) return NotFound();

            try
            {
                await mutate(wallet);
            }
            catch (WalletException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error mutating wallet {WalletId}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }

            return Ok(await _walletService.GetWalletByIdAsync(id, cancellationToken));
        }
    }
}