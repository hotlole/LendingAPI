using Microsoft.EntityFrameworkCore;
using Landing.Core.Models;
using Landing.Infrastructure.Data;
using Landing.Core.Models.Users;

public class AdminSeeder
{
    public static async Task SeedAdminAsync(ApplicationDbContext context)
    {
        // Сначала создаем роли, если их нет
        var requiredRoles = new[] { "Admin", "User", "Moderator" };
        foreach (var roleName in requiredRoles)
        {
            if (!await context.Roles.AnyAsync(r => r.Name == roleName))
            {
                context.Roles.Add(new Role { Name = roleName });
            }
        }
        await context.SaveChangesAsync();

        // Теперь создаем администратора
        string adminEmail = "admin@example.com";
        string adminPassword = "Admin123!";
        var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");

        var adminUser = await context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Email == adminEmail);

        if (adminUser == null)
        {
            adminUser = new User
            {
                FirstName = "Admin",    // Обязательное поле
                LastName = "System",      // Обязательное поле
                MiddleName = "Lending",
                Email = adminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                IsEmailConfirmed = true  // Подтверждаем email для админа
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }

        // Привязываем роль, если еще не привязана
        if (!adminUser.UserRoles.Any(ur => ur.RoleId == adminRole.Id))
        {
            adminUser.UserRoles.Add(new UserRole { RoleId = adminRole.Id });
            await context.SaveChangesAsync();
        }

        Console.WriteLine("✅ Админ создан и добавлен в роль 'Admin'.");
    }
}
