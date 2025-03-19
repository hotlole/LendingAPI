using System.ComponentModel.DataAnnotations;

namespace LandingAPI.DTOs
{
    /// <summary>
    /// DTO для обновления новости.
    /// </summary>
    public class UpdateNewsDto
    {
        /// <summary>
        /// Заголовок новости (макс. 100 символов).
        /// </summary>
        [MaxLength(100, ErrorMessage = "Заголовок не должен превышать 100 символов")]
        public string? Title { get; set; }

        /// <summary>
        /// Описание новости.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Файл изображения (необязательно).
        /// </summary>
        public IFormFile? ImageFile { get; set; }
    }
}
