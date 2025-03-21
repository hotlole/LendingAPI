using Landing.Core.Models;
using Landing.Infrastructure.Data;
using Landing.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Landing.Application.Interfaces;
using Landing.Application.Services;
using System.Reflection;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<NewsService>();
builder.Services.AddScoped<EventService>();
// Настраиваем подключение к PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<UserRepository>();
// Читаем настройки JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);


// Настраиваем аутентификацию через JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
ConfigureDevelopmentServices(builder.Services);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseStaticFiles();
app.Run();
static void
   ConfigureDevelopmentServices(IServiceCollection services)
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    services.AddSwaggerGen(opts =>
    {
        opts.CustomSchemaIds(type => type.FullName);
        opts.IncludeXmlComments(xmlFilePath, true);
        opts.UseAllOfToExtendReferenceSchemas();
        opts.UseAllOfForInheritance();
        opts.UseOneOfForPolymorphism();
        opts.UseInlineDefinitionsForEnums();
        opts.SelectDiscriminatorNameUsing(_ => "$type");
    });

    services.AddEndpointsApiExplorer();
}
// Метод для создания админа
/*void EnsureAdminCreated(ApplicationDbContext context)
{
    if (!context.Users.Any(u => u.Email == "admin@example.com"))
    {
        var admin = new User
        {
            Email = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = "Admin"
        };
        context.Users.Add(admin);
        context.SaveChanges();
    }
}
*/