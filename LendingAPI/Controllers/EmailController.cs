using Landing.Application.Interfaces;
using Landing.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace LendingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController(IEmailService emailService, ApplicationDbContext context) : ControllerBase
    {
        private readonly IEmailService _emailService = emailService;
        private readonly ApplicationDbContext _context = context;

        /// <summary>
        /// Подтверждение email по ссылке из письма.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="token">Токен подтверждения.</param>
        /// <returns>HTML-страница с результатом.</returns>
        [HttpGet("confirm-email")]
        [SwaggerOperation(Summary = "Подтверждение email по токену.")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] int userId, [FromQuery] string token)
        {
            var confirmation = await _context.EmailConfirmationTokens
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Token == token && c.Expires > DateTime.UtcNow);

            if (confirmation == null)
                return HtmlResponse("Ошибка подтверждения", "Ссылка недействительна или срок действия токена истёк.", isSuccess: false);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return HtmlResponse("Пользователь не найден", "Пользователь с указанным ID не существует.", isSuccess: false);

            user.IsEmailConfirmed = true;
            _context.EmailConfirmationTokens.Remove(confirmation);
            await _context.SaveChangesAsync();

            return HtmlResponse("Email подтверждён!", "Теперь вы можете войти в систему.", isSuccess: true);
        }

        // Вспомогательный метод для генерации HTML-ответов
        private ContentResult HtmlResponse(string title, string message, bool isSuccess)
        {
            var bgColor = isSuccess ? "#e0ffe0" : "#ffe5e5";
            var textColor = isSuccess ? "#007e33" : "#b00020";

            var html = $@"
<!DOCTYPE html>
<html lang=""ru"">
<head>
    <meta charset=""UTF-8"">
    <title>{title}</title>
</head>
<body style=""font-family: sans-serif; background-color: {bgColor}; color: {textColor}; padding: 30px;"">
    <h1>{title}</h1>
    <p>{message}</p>
</body>
</html>";

            return Content(html, "text/html");
        }
    }
}
