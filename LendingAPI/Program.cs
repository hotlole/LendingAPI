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
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ��������� �����������
builder.Services.AddControllers()
    .AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<NewsService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<BackgroundTasksService>();
// ����������� ����������� � PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging()  // �������� ����� ������ � ����
           .LogTo(Console.WriteLine, LogLevel.Information));  // �������� ������� � ��
 

builder.Services.AddScoped<UserRepository>();

// ��������� Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// ������ ��������� JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);
// ����������� �������������� ����� JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/admin/login";
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

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

// ��������� Hangfire � ��������� ��������
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();
builder.Services.AddSingleton<BackgroundTasksService>();
builder.Services.AddHostedService<BackgroundTasksService>();
builder.Services.AddHttpClient<VkService>();


builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserTransactionService, UserTransactionService>();



ConfigureDevelopmentServices(builder.Services);

var app = builder.Build();
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
// ����������� Hangfire Dashboard � �������� �����������
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[]
    {
        new HangfireAuthorizationFilter()
    }
});

app.MapGet("/", () => "Hello World!");
// ����������� ������ Hangfire
RecurringJob.AddOrUpdate<BackgroundTasksService>(
    "delete-old-files",
    x => x.DeleteOldFilesAsync(),
    Cron.HourInterval(6)); // ������ 6 �����

RecurringJob.AddOrUpdate<BackgroundTasksService>(
    "award-birthday-points",
    x => x.AwardBirthdayPointsAsync(),
    Cron.Daily); // ���������
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


