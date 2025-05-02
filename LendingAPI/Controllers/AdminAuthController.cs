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

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Role, "Admin")
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        Response.Cookies.Append("hangfire_admin_token", jwt, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(12)
        });

        return Redirect("/hangfire");
    }
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("hangfire_admin_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return Redirect("/admin/login");
    }

}
