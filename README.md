# 🎯 Landing Platform API

> Бэкенд-сервис для управления мероприятиями, пользователями, кураторами и бонусной системой.

---

## 📑 Содержание

- [🚀 Быстрый старт](#-быстрый-старт)
- [🏗️ Архитектура проекта](#️-архитектура-проекта)
- [🔗 Связь кураторов с мероприятиями](#-связь-кураторов-с-мероприятиями)
- [🗃️ Созданные таблицы](#️-созданные-таблицы)
- [🧹 Каскадное удаление](#-каскадное-удаление)
- [🧪 Возможности API](#-возможности-api)
- [📈 Планы на будущее](#-планы-на-будущее)

---

## 🚀 Быстрый старт

```bash
# Настройка базы данных и запуск миграций
dotnet ef database update --startup-project ./LendingAPI --project ./Landing.Infrastructure
Добавь строку подключения в appsettings.json:

json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=landing_db;Username=postgres;Password=your_password"
}
🏗️ Архитектура проекта
ASP.NET Core Web API (.NET 8)

Entity Framework Core + PostgreSQL

JWT-аутентификация с Refresh-токенами

Роли и права доступа (админ, пользователь, куратор)

Пользовательская система баллов

Hangfire для фоновых задач

Blazor WebAssembly (если подключён фронт)

Swagger/OpenAPI для документации

🔗 Связь кураторов с мероприятиями
Кураторы (User) могут быть прикреплены к мероприятиям типа CuratedEvent через таблицу UserCuratedEvents (many-to-many).

Модель: User
csharp

public virtual ICollection<CuratedEvent> CuratedEvents { get; set; }
Модель: CuratedEvent
csharp

public virtual ICollection<User> Curators { get; set; }
Конфигурация
csharp

builder
    .HasMany(e => e.Curators)
    .WithMany(u => u.CuratedEvents)
    .UsingEntity<Dictionary<string, object>>(
        "UserCuratedEvents",
        j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade),
        j => j.HasOne<CuratedEvent>().WithMany().HasForeignKey("CuratedEventId").OnDelete(DeleteBehavior.Cascade),
        j =>
        {
            j.HasKey("UserId", "CuratedEventId");
            j.ToTable("UserCuratedEvents");
        });
🗃️ Созданные таблицы
Таблица	Назначение
Users	Пользователи системы
Events	Базовые данные о мероприятиях
CuratedEvents	Мероприятия с кураторами
UserCuratedEvents	Таблица связей пользователей и мероприятий
UserRoles	Роли пользователей
EventAttendances	Посещения мероприятий
UserPointsTransactions	История начислений баллов
RefreshTokens	Обновляющие JWT токены
News	Новости/объявления на платформе
🧹 Каскадное удаление
При удалении пользователя — удаляются все связи с кураторскими мероприятиями (UserCuratedEvents)

При удалении мероприятия — все связи с кураторами также удаляются

Это задаётся через OnDelete(DeleteBehavior.Cascade) в конфигурации связи

🧪 Возможности API
✅ Регистрация и подтверждение email

✅ Авторизация через JWT и Refresh Tokens

✅ Роли и разграничение доступа

✅ CRUD для новостей и мероприятий

✅ Запись на мероприятия

✅ Назначение кураторов

✅ Подтверждение явки и начисление баллов

✅ Расчёт баллов с учётом транзакций

✅ Задания по расписанию (через Hangfire)
