﻿using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Landing.Infrastructure.Data;
using Landing.Infrastructure.Services;
using Landing.Core.Models;
using Landing.Infrastructure.Repositories;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Landing.Application.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Cryptography;

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
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                                      .FirstOrDefault(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Неверный email или пароль");

            var role = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "User";
            var token = GenerateJwtToken(user, role);
            var refreshToken = GenerateRefreshToken(user.Id);

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return Ok(new { Token = token, RefreshToken = refreshToken.Token });
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (storedToken == null || !storedToken.IsActive)
                return Unauthorized("Недействительный или просроченный рефреш-токен.");

            var user = await _context.Users.FindAsync(storedToken.UserId);
            if (user == null)
                return Unauthorized("Пользователь не найден.");

            var role = user.UserRoles.FirstOrDefault()?.Role?.Name ?? "User";
            var newJwt = GenerateJwtToken(user, role);
            var newRefreshToken = GenerateRefreshToken(user.Id);

            storedToken.Revoked = DateTime.UtcNow;
            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return Ok(new { Token = newJwt, RefreshToken = newRefreshToken.Token });
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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                BirthDate = request.BirthDate, 
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("Пользователь зарегистрирован. Подтвердите email по ссылке в письме.");
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
            Console.WriteLine($"Secret key length: {secretKey.Length}");

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
        /// <summary>
        /// Смена роли пользователя (только для администраторов).
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="newRole">Новая роль</param>
        /// <returns>Результат операции</returns>
        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeUserRole([FromQuery] int userId, [FromQuery] string newRole)
        {
            // Проверка прав: только администратор может менять роль
            var currentUser = _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                                            .FirstOrDefault(u => u.Email == User.Identity.Name); // Идентификатор текущего пользователя из токена
            if (currentUser == null || !currentUser.UserRoles.Any(ur => ur.Role.Name == "Admin"))
            {
                return Unauthorized("У вас нет прав для изменения роли.");
            }

            var user = await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                                           .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound("Пользователь не найден");

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == newRole);
            if (role == null)
                return BadRequest("Неверная роль");

            var userRole = user.UserRoles.FirstOrDefault();
            if (userRole != null)
            {
                userRole.RoleId = role.Id;
            }
            else
            {
                userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                };
                _context.UserRoles.Add(userRole);
            }

            await _context.SaveChangesAsync();

            return Ok("Роль пользователя изменена");
        }
        private RefreshToken GenerateRefreshToken(int userId)
        {
            return new RefreshToken
            {
                UserId = userId,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };
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

        [Required(ErrorMessage = "Дата рождения обязательна")]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1900-01-01", "2100-01-01", ErrorMessage = "Некорректная дата рождения")]
        public DateTime BirthDate { get; set; }
    }
    public class RefreshRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
