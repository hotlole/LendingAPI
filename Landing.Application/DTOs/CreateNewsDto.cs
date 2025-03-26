using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Landing.Application.DTOs
{
    
    public class CreateNewsDto
    {
        
        [Required(ErrorMessage = "Заголовок обязателен")]
        [MaxLength(100, ErrorMessage = "Заголовок не должен превышать 100 символов")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Описание обязательно")]
        public string Description { get; set; }
        public IFormFile? ImageFile { get; set; }

    }
}
