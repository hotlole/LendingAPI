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
using Microsoft.OpenApi.Models;
using Landing.Infrastructure.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<NewsService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<BackgroundTasksService>();
// Настраиваем подключение к PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging()  // Включает вывод данных в логи
           .LogTo(Console.WriteLine, LogLevel.Information));  // Логирует запросы к БД
builder.Services.AddScoped<UserRepository>();

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

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
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
ConfigureDevelopmentServices(builder.Services);

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    /*await AdminSeeder.SeedAdminAsync(context);*/

    if (!context.Roles.Any())
    {
        context.Roles.AddRange(new List<Role>
        {
            new Role { Name = "Admin" },
            new Role { Name = "User" },
            new Role { Name = "Moderator" }
        });

        context.SaveChanges();
    }
}

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
static void ConfigureDevelopmentServices(IServiceCollection services)
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    services.AddSwaggerGen(opts =>
    {

        
        opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter your Bearer token",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });

        opts.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new List<string>()
            }
        });

        
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


