using Landing.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LendingAPI.Controllers
{
    /// <summary>
    /// Контроллер для управления транзакциями пользователей, включая добавление и вычитание баллов.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserTransactionService transactionService) : ControllerBase
    {
        private readonly IUserTransactionService _transactionService = transactionService;
       
        /// <summary>
        /// Добавление баллов пользователю.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="points">Количество добавляемых баллов</param>
        /// <returns>Статус успешной операции</returns>
        /// <response code="200">Баллы успешно добавлены</response>
        /// <response code="400">Некорректный запрос</response>
        [HttpPost("{userId}/add-points")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddPoints(int userId, [FromBody] int points)
        {
            await _transactionService.AddPointsAsync(userId, points, "Manual points addition");
            return Ok();
        }

        /// <summary>
        /// Вычитание баллов у пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="points">Количество вычитаемых баллов</param>
        /// <returns>Статус успешной операции</returns>
        /// <response code="200">Баллы успешно вычтены</response>
        /// <response code="400">Некорректный запрос</response>
        [HttpPost("{userId}/subtract-points")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubtractPoints(int userId, [FromBody] int points)
        {
            await _transactionService.SubtractPointsAsync(userId, points, "Manual points subtraction");
            return Ok();
        }

        /// <summary>
        /// Получение всех транзакций пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Список транзакций пользователя</returns>
        /// <response code="200">Список транзакций успешно получен</response>
        /// <response code="404">Пользователь не найден</response>
        [HttpGet("{userId}/transactions")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserTransactions(int userId)
        {
            var transactions = await _transactionService.GetUserTransactionsAsync(userId);
            if (transactions == null || !transactions.Any())
                return NotFound("Транзакции для данного пользователя не найдены.");
            return Ok(transactions);
        }
    }

}
