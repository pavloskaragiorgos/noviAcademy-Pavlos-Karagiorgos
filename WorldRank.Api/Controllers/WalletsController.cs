using Microsoft.AspNetCore.Mvc;
using WorldRank.Api.Dtos;
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

        [HttpPost("{id:int}/deposit")]
        public async Task<IActionResult> Deposit(int id, [FromBody] DepositRequest request, CancellationToken cancellationToken)
        {
            var wallet = await _walletService.GetWalletByIdAsync(id, cancellationToken);
            if (wallet is null) return NotFound();

            try
            {
                await _walletService.DepositToWalletAsync(wallet.PlayerId, wallet.Currency, request.Amount, cancellationToken);
            }
            catch (WalletException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error depositing to wallet {WalletId}", id);
                return StatusCode(500, "An unexpected error occurred.");
            }

            return Ok(await _walletService.GetWalletByIdAsync(id, cancellationToken));
        }
    }
}