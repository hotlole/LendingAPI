using FluentValidation;

namespace Landing.Application.Validators;
/// <summary>
/// Запрос на обновление Refresh-токена.
/// </summary>
public class RefreshRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
public class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh-токен обязателен");
    }
}
