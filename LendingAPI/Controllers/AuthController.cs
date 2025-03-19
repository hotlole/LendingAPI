using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Landing.Infrastructure.Data;
using Landing.Core.Models;
using Landing.Infrastructure.Repositories;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LandingAPI.Controllers
{
    /// <summary>
    /// Контроллер для авторизации и регистрации пользователей.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly UserRepository _userRepository;



        /// <summary>
        /// Конструктор контроллера авторизации.
        /// </summary>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="userRepository">Репозиторий пользователей</param>
        public AuthController(IConfiguration configuration, ApplicationDbContext context, UserRepository userRepository)
        {
            _configuration = configuration;
            _context = context;
            _userRepository = userRepository;
        }
        /// <summary>
        /// Авторизация пользователя.
        /// </summary>
        /// <param name="request">Данные для входа</param>
        /// <returns>JWT-токен, если авторизация успешна</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                                      .FirstOrDefault(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Неверный email или пароль");

            var role = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "User";
            var token = GenerateJwtToken(user, role);
            return Ok(new { Token = token });
        }
        /// <summary>
        /// Регистрация нового пользователя.
        /// </summary>
        /// <param name="request">Данные пользователя</param>
        /// <returns>Сообщение о статусе регистрации</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest("Пользователь с таким email уже существует");

            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var userRole = new UserRole
            {
                UserId = newUser.Id,
                RoleId = _context.Roles.FirstOrDefault(r => r.Name == "User")?.Id ?? 1
            };
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return Ok("Пользователь зарегистрирован");
        }
        /// <summary>
        /// Генерация JWT-токена.
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="role">Роль пользователя</param>
        /// <returns>JWT-токен</returns>
        private string GenerateJwtToken(User user, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["TokenLifetimeMinutes"])),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    /// <summary>
    /// Запрос на авторизацию.
    /// </summary>
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    /// <summary>
    /// Запрос на регистрацию.
    /// </summary>
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        public string Password { get; set; }
    }
}
