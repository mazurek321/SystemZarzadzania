using System.Text;
using Infrastructure.Context;
using Infrastructure.Database;
using Infrastructure.Database.Abstractions;
using Infrastructure.Email;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Configuration;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Core.Models.Users;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/app-log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
        };
    });

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<AppDbContext>((serviceProvider, optionsBuilder) =>
{
    optionsBuilder.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 27))
    );
});

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddHttpContextAccessor();
builder.Services.AddRepositories();
builder.Services.AddServices();

builder.Services.Configure<EmailInfo>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<Core.Models.Notifications.INotificationSender, Api.Services.NotificationSender>();

builder.Services.AddQuartzJobs();

builder.Services.AddTransient<ISqlConnectionFactory, SqlConnectionFactory>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Infrastructure.EventHandlers.UserRegisteredEventHandler>();
    cfg.RegisterServicesFromAssemblyContaining<Core.Events.UserRegisteredEvent>();
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevServer",
        builder => builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

app.UseCors("AllowAngularDevServer");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    var userRepository = services.GetRequiredService<IUserRepository>();
    var seeder = new AdminSeeder(context, userRepository);
    await seeder.SeedAdminAsync();
}

app.MapControllers();
app.MapHub<Api.Hubs.NotificationHub>("/notificationHub");

app.Run();

public partial class Program { }
