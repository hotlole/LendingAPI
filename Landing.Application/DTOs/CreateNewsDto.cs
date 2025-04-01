using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Landing.Application.DTOs
{
    /// <summary>
    /// Модель для создания новостей.
    /// </summary>
    [SwaggerSchema("Модель, описывающая данные для создания новостей.")]
    public class CreateNewsDto
    {
        /// <summary>
        /// Заголовок новости.
        /// </summary>
        [Required(ErrorMessage = "Заголовок обязателен")]
        [MaxLength(100, ErrorMessage = "Заголовок не должен превышать 100 символов")]
        [SwaggerSchema("Заголовок новости. Обязательное поле, не более 100 символов.")]
        public string Title { get; set; }

        /// <summary>
        /// Описание новости.
        /// </summary>
        [Required(ErrorMessage = "Описание обязательно")]
        [SwaggerSchema("Описание новости. Обязательное поле.")]
        public string Description { get; set; }

        /// <summary>
        /// Изображение новости.
        /// </summary>
        [SwaggerSchema("Изображение новости (файл). Это поле не обязательно.")]
        public IFormFile? ImageFile { get; set; }
    }
}
