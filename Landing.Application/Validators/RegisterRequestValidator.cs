using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Landing.Application.Validators
{
    /// <summary>
    /// Запрос на регистрацию.
    /// </summary>
    public class RegisterRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [DataType(DataType.Date)] 
        public DateTime? BirthDate { get; set; } 

    }

    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Имя обязательно")
                .MaximumLength(100).WithMessage("Имя не должно превышать 100 символов");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Фамилия обязательна")
                .MaximumLength(100).WithMessage("Фамилия не должна превышать 100 символов");

            RuleFor(x => x.MiddleName)
                .MaximumLength(100).WithMessage("Отчество не должно превышать 100 символов");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен")
                .EmailAddress().WithMessage("Некорректный email");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(6).WithMessage("Минимум 6 символов");

            RuleFor(x => x.BirthDate)
               .NotEmpty().WithMessage("Дата рождения обязательна")
               .Must(BeValidDate).WithMessage("Неверный формат даты (Ожидается yyyy-MM-dd)");
        }

        private bool BeValidDate(DateTime? birthdate)
        {
            return birthdate.HasValue && birthdate.Value.ToString("yyyy-MM-dd") == birthdate.Value.ToString("yyyy-MM-dd");
        }
    }
}
