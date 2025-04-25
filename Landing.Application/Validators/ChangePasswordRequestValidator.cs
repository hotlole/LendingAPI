using FluentValidation;

namespace Landing.Application.Validators
{
    /// <summary>
    /// Запрос на смену пароля.
    /// </summary>
    public class ChangePasswordRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Некорректный email");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(6).WithMessage("Пароль должен содержать хотя бы 6 символов.");
        }
    }
}
