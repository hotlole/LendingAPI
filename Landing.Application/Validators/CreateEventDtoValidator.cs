using FluentValidation;
using Landing.Application.DTOs.Events;
using Landing.Core.Models.Events;

namespace Landing.Application.Validators
{
    public class CreateEventDtoValidator : AbstractValidator<CreateEventDto>
    {
        public CreateEventDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Название мероприятия обязательно")
                .MaximumLength(200).WithMessage("Максимум 200 символов");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Описание обязательно");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Дата мероприятия обязательна")
                .GreaterThan(DateTime.MinValue).WithMessage("Некорректная дата");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Тип мероприятия должен быть валидным значением перечисления EventType");

            When(x => x.Type == EventType.Offline, () =>
            {
                RuleFor(x => x.Address)
                    .NotEmpty().WithMessage("Адрес обязателен для очного мероприятия");

                RuleFor(x => x.Latitude)
                    .NotNull().WithMessage("Широта обязательна для очного мероприятия");

                RuleFor(x => x.Longitude)
                    .NotNull().WithMessage("Долгота обязательна для очного мероприятия");

                RuleFor(x => x.CustomHtmlTemplate)
                    .NotEmpty().WithMessage("HTML-шаблон обязателен для очного мероприятия");
            });

            When(x => x.Type == EventType.Curated, () =>
            {
                RuleFor(x => x.CuratorIds)
                    .NotNull().WithMessage("Список кураторов обязателен для курируемого мероприятия")
                    .Must(list => list!.Count > 0).WithMessage("Нужно указать хотя бы одного куратора");
            });
        }
    }
}
