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

        public WalletsController(AppWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost]
        public IActionResult CreateWallet([FromBody] CreateWalletRequest request)
        {
            try
            {
                var wallet = _walletService.AddWalletToPlayer(request.PlayerId, request.Currency, request.Balance);
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
        }

        [HttpGet("{id:int}")]
        public IActionResult GetWalletById(int id)
        {
            var wallet = _walletService.GetWalletById(id);
            if (wallet is null) return NotFound();

            return Ok(wallet);
        }

        [HttpPost("{id:int}/deposit")]
        public IActionResult Deposit(int id, [FromBody] DepositRequest request)
        {
            var wallet = _walletService.GetWalletById(id);
            if (wallet is null) return NotFound();

            try
            {
                _walletService.DepositToWallet(wallet.PlayerId, wallet.Currency, request.Amount);
            }
            catch (WalletException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(_walletService.GetWalletById(id));
        }
    }
}