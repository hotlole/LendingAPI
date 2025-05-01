using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Landing.Infrastructure.Data;
using Landing.Core.Models;
using Landing.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Landing.Application.Interfaces;
using System.Security.Cryptography;
using Landing.Core.Models.Users;
using System.Web;
using Landing.Application.Validators;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        /// <summary>
        /// Конструктор контроллера авторизации.
        /// </summary>
        /// <param name="configuration">Конфигурация приложения</param>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="userRepository">Репозиторий пользователей</param>
        public AuthController(IConfiguration configuration, ApplicationDbContext context, IUserRepository userRepository, IEmailService emailService)
        {
            _configuration = configuration;
            _context = context;
            _userRepository = userRepository;
            _emailService = emailService;
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
        /// <summary>
        /// Обновление JWT токена с использованием Refresh токена.
        /// </summary>
        /// <param name="request">Запрос с Refresh токеном</param>
        /// <returns>Новый JWT-токен и новый Refresh-токен</returns>
        /// <response code="200">Возвращает новый JWT-токен и Refresh-токен</response>
        /// <response code="401">Неверный или просроченный Refresh токен</response>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (storedToken == null || !storedToken.IsActive)
                return Unauthorized("Недействительный или просроченный рефреш-токен.");

            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == storedToken.UserId);

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

            string birthDateString = request.BirthDate?.ToString("yyyy-MM-dd");
            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                BirthDate = DateTime.TryParseExact(birthDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate)
        ? parsedDate
        : (DateTime?)null,
                IsEmailConfirmed = false
            }; 


            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Создание токена подтверждения
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var emailToken = new EmailConfirmationToken
            {
                Token = token,
                UserId = newUser.Id,
                Expires = DateTime.UtcNow.AddHours(24)
            };
            _context.EmailConfirmationTokens.Add(emailToken);
            await _context.SaveChangesAsync();

            // Отправка письма
            var scheme = Request?.Scheme ?? "https";
            var confirmationLink = Url.Action(
                "ConfirmEmail",
                "Email",
                new { userId = newUser.Id, token },
                scheme);
            /*var confirmationLink = $"http://localhost:5164/api/email/confirm-email?userId={newUser.Id}&token={HttpUtility.UrlEncode(token)}";*/
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendEmailConfirmationAsync(newUser.Email, confirmationLink);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при отправке email: {ex.Message}");
                }
            });
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
        [Authorize(Roles = "Admin")]
        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeUserRole([FromQuery] int userId, [FromQuery] string newRole)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var currentUser = await _context.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (currentUser == null || !currentUser.UserRoles.Any(ur => ur.Role.Name == "Admin"))
                return Unauthorized("У вас нет прав для изменения роли.");

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
        /// <summary>
        /// Смена пароля пользователя.
        /// </summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            // Проверка, что email подтверждён
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.IsEmailConfirmed);
            if (user == null)
            {
                return Unauthorized("Email не подтверждён.");
            }

            // Валидируем весь запрос с помощью Fluent Validation
            var passwordValidator = new ChangePasswordRequestValidator();
            var validationResult = passwordValidator.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            // Хешируем новый пароль
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            return Ok("Пароль успешно изменён.");
        }

        /// <summary>
        /// Генерация Refresh-токена для пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Объект Refresh-токена</returns>
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
    
}
