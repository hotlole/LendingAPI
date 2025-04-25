using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Landing.Infrastructure.Data;

namespace LandingAPI.Controllers;

[Route("admin")]
[ApiExplorerSettings(IgnoreApi = true)]
public class AdminAuthController(ApplicationDbContext db, IConfiguration config) : Controller
{
    private readonly ApplicationDbContext _db = db;
    private readonly IConfiguration _config = config;

    [HttpGet("login")]
    public IActionResult Login()
    {
        var html = """
            <form method="post" action="/admin/login">
                <label>Email: <input type="email" name="email" /></label><br/>
                <label>Password: <input type="password" name="password" /></label><br/>
                <button type="submit">Login</button>
            </form>
        """;
        return Content(html, "text/html");
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginPost([FromForm] string email, [FromForm] string password)
    {
        var user = await _db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return Unauthorized("Неверный email или пароль");

        var isAdmin = user.UserRoles.Any(ur => ur.Role.Name == "Admin");
        if (!isAdmin)
            return Unauthorized("Доступ только для администраторов");

        // Выдаем JWT токен
        var claims = new[] {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Role, "Admin")
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        // Создаем ссылку на Hangfire с Bearer токеном
        var hangfireUrl = $"{Request.Scheme}://{Request.Host}/hangfire";
        var html = $"""
    <p>Авторизация прошла успешно.</p>
    <p>Вот ваш токен:</p>
    <textarea style="width:100%;height:200px;">{jwt}</textarea>
    <p>Вставьте его в заголовок Authorization: Bearer &lt;token&gt; при заходе в Hangfire</p>
    <p>Перейти в <a href="{hangfireUrl}" target="_blank">Hangfire</a> (с токеном)</p>
    <p>Для авторизации в Hangfire добавьте токен в заголовок Authorization: Bearer {jwt}</p>
    """;

        return Content(html, "text/html", Encoding.UTF8);
    }



    [HttpGet("logout")]
    public IActionResult Logout()
    {
        return Redirect("/admin/login");
    }
}
