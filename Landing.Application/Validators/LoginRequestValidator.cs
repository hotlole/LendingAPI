using FluentValidation;

namespace Landing.Application.Validators
{
    /// <summary>
    /// Запрос на авторизацию.
    /// </summary>
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Некорректный email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен");
        }
    }
}
