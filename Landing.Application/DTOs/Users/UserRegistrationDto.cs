using System.ComponentModel.DataAnnotations;

namespace Landing.Application.DTOs.Users
{
    public class UserRegistrationDto
    {
        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1900-01-01", "2100-01-01", ErrorMessage = "Некорректная дата рождения.")]
        public DateTime BirthDate { get; set; }

    }

}
