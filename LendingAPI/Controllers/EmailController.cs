using DocumentFormat.OpenXml.InkML;
using Landing.Application.Interfaces;
using Landing.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace LendingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;

        public EmailController(IEmailService emailService, ApplicationDbContext context)
        {
            _emailService = emailService;
            _context = context;
        }

        /// <summary>
        /// Отправка письма для подтверждения email.
        /// </summary>
        /// <param name="email">Адрес электронной почты получателя.</param>
        /// <param name="confirmationLink">Ссылка для подтверждения.</param>
        /// <returns>Результат операции.</returns>
        /// <response code="200">Письмо успешно отправлено.</response>
        /// <response code="500">Ошибка при отправке письма.</response>
        [HttpPost("send-confirmation")]
        [SwaggerOperation(Summary = "Отправка письма для подтверждения email.")]
        [SwaggerResponse(200, "Письмо успешно отправлено.")]
        [SwaggerResponse(500, "Ошибка при отправке письма.")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] int userId, [FromQuery] string token)
        {
            var confirmation = await _context.EmailConfirmationTokens
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Token == token && c.Expires > DateTime.UtcNow);

            if (confirmation == null)
                return BadRequest("Недействительный или просроченный токен.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Пользователь не найден.");

            user.IsEmailConfirmed = true;
            _context.EmailConfirmationTokens.Remove(confirmation);

            await _context.SaveChangesAsync();

            return Ok("Email успешно подтвержден.");
        }
    }
}
