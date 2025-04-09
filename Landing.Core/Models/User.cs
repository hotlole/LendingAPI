using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Landing.Core.Models.Events;

namespace Landing.Core.Models
{
    /// <summary>
    /// Представляет пользователя системы.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Уникальный идентификатор пользователя.
        /// </summary>
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string MiddleName { get; set; } = string.Empty;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(300)]
        public string FullName { get; private set; } = string.Empty;

        /// <summary>
        /// Адрес электронной почты пользователя.
        /// </summary>
        /// <example>example@domain.com</example>
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [MaxLength(255, ErrorMessage = "Максимальная длина email 255 символов")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Подтвержден ли email пользователя.
        /// </summary>
        public bool IsEmailConfirmed { get; set; } = false;

        /// <summary>
        /// Хеш пароля пользователя.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Роли пользователя.
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();

        /// <summary>
        /// События, на которые пользователь записался.
        /// </summary>
        public ICollection<EventAttendance> Attendances { get; set; } = new HashSet<EventAttendance>();

        /// <summary>
        /// Рефреш-токены пользователя.
        /// </summary>
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        /// <summary>
        /// Транзакции баллов пользователя.
        /// </summary>
        public ICollection<UserPointsTransaction> PointsTransactions { get; set; } = new List<UserPointsTransaction>();

        /// <summary>
        /// События, которые пользователь курирует.
        /// </summary>
        public ICollection<CuratedEvent> CuratedEvents { get; set; } = new List<CuratedEvent>();

        /// <summary>
        /// Баллы пользователя (вычисляемое свойство).
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int Points { get; set; }

        /// <summary>
        /// Дата рождения пользователя.
        /// </summary>
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1900-01-01", "2100-01-01", ErrorMessage = "Некорректная дата рождения")]
        public DateTime? BirthDate { get; set; }
    }
}
