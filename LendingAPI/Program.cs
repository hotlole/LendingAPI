using Landing.Core.Models;
using Landing.Infrastructure.Data;
using Landing.Infrastructure.Repositories;
using Landing.Infrastructure.Services;
using Landing.Application.Interfaces;
using Landing.Application.Services;
using Landing.Application.Mappings;
using Landing.Application.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;
using Landing.Core.Models.Events;
using Landing.Core.Models.News;
using Refit;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using LandingAPI.Extensions;
var builder = WebApplication.CreateBuilder(args);

// --- Конфигурация логгирования через Serilog ---
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// --- Добавляем сервисы ---
builder.Services.AddControllers()
    .AddNewtonsoftJson();
builder.Services.ConfigureHealthChecks(builder.Configuration);
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-API-Version"),
        new QueryStringApiVersionReader("api-version")
    );
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// --- Swagger ---
builder.Services.ConfigureSwagger();

// --- Подключение к БД ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information));

// --- Репозитории ---
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// --- Сервисы ---
builder.Services.AddScoped<NewsService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<BackgroundTasksService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserTransactionService, UserTransactionService>();
builder.Services.AddScoped<FileCleanupService>();
builder.Services.AddScoped<ImageCompressionService>();
builder.Services.AddRefitClient<IVkApiClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri("https://api.vk.com");
    });

builder.Services.AddTransient<VkService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<RelativePathResolver<Event>>();
builder.Services.AddTransient<RelativePathResolver<RegularEvent>>();
builder.Services.AddTransient<RelativePathResolver<CuratedEvent>>();
builder.Services.AddTransient<RelativePathResolver<OfflineEvent>>();
builder.Services.AddTransient<RelativePathResolver<News>>();
// --- Автоматическое маппинг профилей ---
builder.Services.AddAutoMapper(cfg =>
{
    cfg.ConstructServicesUsing(type => builder.Services.BuildServiceProvider().GetRequiredService(type));
}, AppDomain.CurrentDomain.GetAssemblies());

// --- FluentValidation ---
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEventDtoValidator>();
// --- Кастомизация ответа при ошибках валидации ---
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value.Errors.Any())
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        return new BadRequestObjectResult(new
        {
            message = "Ошибка валидации",
            errors
        });
    };
});


// --- Аутентификация через JWT ---
ConfigureAuthentication(builder.Services, builder.Configuration);

// --- Hangfire ---
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

// --- Авторизация ---
builder.Services.AddAuthorization();

var app = builder.Build();

// --- Инициализация базы данных ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

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

    await AdminSeeder.SeedAdminAsync(context);
}

// --- Middleware ---
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLowerInvariant();

    // Только на сам путь /hangfire и его корень
    if (path != null && (path == "/hangfire" || path == "/hangfire/"))
    {
        var token = context.Request.Query["access_token"].FirstOrDefault();
        if (!string.IsNullOrEmpty(token))
        {
            context.Request.Headers["Authorization"] = $"Bearer {token}";
        }
    }

    await next();
});


app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"API {description.GroupName.ToUpperInvariant()}");
        }
        options.HeadContent = """<link rel="stylesheet" href="/css/swagger-custom.css">""";
    });
}
app.MapControllers();


// --- Hangfire Dashboard ---
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter(builder.Configuration) },
    AppPath = "/admin/login"
});

// --- HealthChecks ---
app.UseHealthChecksAndUI();
app.MapGet("/monitoring", context =>
{
    context.Response.Redirect("/health-ui", permanent: false);
    return Task.CompletedTask;
});

// --- Планировщик задач ---
RecurringJob.AddOrUpdate<FileCleanupService>(
    "cleanup-files",
    service => service.CleanupOrphanedFilesAsync(3),
    Cron.Daily);

RecurringJob.AddOrUpdate<BackgroundTasksService>(
    "award-birthday-points",
    x => x.AwardBirthdayPointsAsync(),
    Cron.Daily);

app.MapGet("/", () => "Hello World!");

app.Run();

// --- Конфигурация аутентификации ---
static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
{
    var jwtSettings = configuration.GetSection("JwtSettings");
    var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
    });
}


