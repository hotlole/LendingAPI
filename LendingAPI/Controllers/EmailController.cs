using Landing.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LendingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
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
        public async Task<IActionResult> SendEmailConfirmation([FromQuery] string email, [FromQuery] string confirmationLink)
        {
            try
            {
                await _emailService.SendEmailConfirmationAsync(email, confirmationLink);
                return Ok("Письмо с подтверждением отправлено!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при отправке письма: {ex.Message}");
            }
        }
    }
}
