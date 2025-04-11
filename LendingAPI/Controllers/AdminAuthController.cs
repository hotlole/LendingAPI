using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Landing.Infrastructure.Data;

namespace LandingAPI.Controllers
{
    [Route("admin")]
    public class AdminAuthController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminAuthController(ApplicationDbContext db)
        {
            _db = db;
        }

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

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return Redirect("/hangfire");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/admin/login");
        }
    }
}
