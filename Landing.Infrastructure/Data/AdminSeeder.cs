using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Landing.Core.Models;
using Landing.Infrastructure.Data;

public class AdminSeeder
{
    public static async Task SeedAdminAsync(ApplicationDbContext context)
    {
        string adminEmail = "admin@example.com";
        string adminPassword = "Admin123!"; // Можно захешировать перед сохранением

        
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        if (adminRole == null)
        {
            adminRole = new Role { Name = "Admin" };
            context.Roles.Add(adminRole);
            await context.SaveChangesAsync();
        }

        
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
        if (adminUser == null)
        {
            adminUser = new User
            {
                Id = 0,
                Email = adminEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword) // Хешируем пароль
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }

        
        var userRole = await context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id);
        if (userRole == null)
        {
            userRole = new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id };
            context.UserRoles.Add(userRole);
            await context.SaveChangesAsync();
        }

        Console.WriteLine("✅ Админ создан и добавлен в роль 'Admin'.");
    }
}
