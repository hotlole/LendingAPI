## 🎯 Landing Platform API

Бэкенд-сервис для управления мероприятиями, пользователями, кураторами и бонусной системой.

---

## 📑 Содержание

- 🚀 Быстрый старт
- 🏗️ Архитектура проекта
- 🔗 Связь кураторов с мероприятиями
- 🗃️ Созданные таблицы
- 🧹 Каскадное удаление
- 🧪 Возможности API


---

## 🚀 Быстрый старт

### 🔧 Настройка базы данных и миграций

```bash
dotnet ef database update --startup-project ./LendingAPI --project ./Landing.Infrastructure
```

Добавь строку подключения в `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=landing_db;Username=postgres;Password=your_password"
}
```

---

## 🏗️ Архитектура проекта

- **ASP.NET Core Web API (.NET 8)**
- **Entity Framework Core + PostgreSQL**
- **JWT-аутентификация с Refresh-токенами**
- **Роли и разграничение прав доступа** (пользователь, админ, куратор)
- **Система баллов с транзакциями**
- **Hangfire** — для фоновых и отложенных задач
- **Swagger / OpenAPI** — встроенная документация

---

## 🔗 Связь кураторов с мероприятиями

Кураторы (`User`) могут быть прикреплены к мероприятиям типа `CuratedEvent`. Связь реализована через промежуточную таблицу `UserCuratedEvents` (many-to-many).

**Модель `User`:**
```csharp
public virtual ICollection<CuratedEvent> CuratedEvents { get; set; }
```

**Модель `CuratedEvent`:**
```csharp
public virtual ICollection<User> Curators { get; set; }
```

**Fluent API конфигурация:**
```csharp
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
```

---

## 🗃️ Созданные таблицы

| Таблица                | Назначение                                      |
|------------------------|--------------------------------------------------|
| `Users`                | Пользователи системы                            |
| `Events`               | Общая информация о мероприятиях                |
| `CuratedEvents`        | Мероприятия с кураторами (наследует `Event`)   |
| `UserCuratedEvents`    | Связи между кураторами и мероприятиями         |
| `UserRoles`            | Роли пользователей                             |
| `EventAttendances`     | Учёт посещаемости мероприятий                  |
| `UserPointsTransactions`| Начисление и списание баллов                  |
| `RefreshTokens`        | Обновляющие JWT-токены                         |
| `News`                 | Новости и объявления                           |

---

## 🧹 Каскадное удаление

- При **удалении пользователя**: связи с мероприятиями в `UserCuratedEvents` удаляются автоматически
- При **удалении мероприятия**: все прикреплённые кураторы тоже отвязываются

Это обеспечивается с помощью `OnDelete(DeleteBehavior.Cascade)` в конфигурации модели.

---

## 🧪 Возможности API

✅ Регистрация и подтверждение Email

✅ Аутентификация через JWT + Refresh Tokens

✅ Роли: администратор, пользователь, куратор

✅ CRUD для новостей и мероприятий

✅ Запись на мероприятия и подтверждение явки

✅ Назначение кураторов для мероприятий

✅ Автоматическое начисление баллов и учёт транзакций

✅ Отложенные задания (Hangfire):
- Удаление мусорных файлов
- Начисление баллов на день рождения

---


