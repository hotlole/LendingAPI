using System.ComponentModel.DataAnnotations;
namespace Landing.Application.DTOs
{
    public class UserUpdateDto
    {
        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; } 
    }

}
