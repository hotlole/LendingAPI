using Landing.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LendingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserTransactionService _transactionService;

        public UserController(IUserTransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("{userId}/add-points")]
        public async Task<IActionResult> AddPoints(int userId, [FromBody] int points)
        {
            await _transactionService.AddPointsAsync(userId, points, "Manual points addition");
            return Ok();
        }

        [HttpPost("{userId}/subtract-points")]
        public async Task<IActionResult> SubtractPoints(int userId, [FromBody] int points)
        {
            await _transactionService.SubtractPointsAsync(userId, points, "Manual points subtraction");
            return Ok();
        }

        [HttpGet("{userId}/transactions")]
        public async Task<IActionResult> GetUserTransactions(int userId)
        {
            var transactions = await _transactionService.GetUserTransactionsAsync(userId);
            return Ok(transactions);
        }
    }

}
