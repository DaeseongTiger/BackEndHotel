using ForMiraiProject.Services.Interfaces;
using ForMiraiProject.Services;
using ForMiraiProject.Repositories.Interfaces;
using ForMiraiProject.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ForMiraiProject.Models;
using ForMiraiProject.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ForMiraiProject API",
        Version = "v1",
        Description = "API for ForMiraiProject",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "ForMiraiProject Support",
            Email = "support@formiraiproject.com"
        }
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your Bearer token below:"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing.")))
    };
});

// Configure Database Context (AppDbContext)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));  // เชื่อมต่อกับฐานข้อมูล

// Dependency Injection for services and repositories
builder.Services.AddLogging();
builder.Services.AddScoped<I_WeatherService, WeatherService>();
builder.Services.AddScoped<I_AuthService, AuthService>();
builder.Services.AddScoped<I_AuthRepository, AuthRepository>();
builder.Services.AddScoped<I_BookingService, BookingService>();
builder.Services.AddScoped<I_BookingRepository, BookingRepository>();
builder.Services.AddScoped<I_FeedbackService, FeedbackService>(); // เพิ่ม FeedbackService
builder.Services.AddScoped<I_FeedbackRepository, FeedbackRepository>(); // เพิ่ม FeedbackRepository
builder.Services.AddScoped<I_HotelRepository, HotelRepository>();
builder.Services.AddScoped<I_UserRepository, UserRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddScoped<I_AuthService, AuthService>();


// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.WithOrigins("https://trusteddomain.com") // Change to allowed origins
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ForMiraiProject API v1");
        options.RoutePrefix = string.Empty;
    });
}

// Middleware for Error Handling
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while processing the request. Request path: {Path}", context.Request.Path);
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
    }
});

// Enable Authentication & Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

// Enable HTTPS redirection in all environments
app.UseHttpsRedirection();

// Apply CORS Policy
app.UseCors("DefaultPolicy");

// Map Controllers
app.MapControllers();

// Map Weather API
app.MapGet("/weatherforecast", async (I_WeatherService weatherService, ILogger<Program> logger) =>
{
    logger.LogInformation("Fetching weather forecast data...");
    var forecast = await weatherService.GetWeatherForecastAsync(new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" });
    return Results.Ok(forecast);
})
.WithName("GetWeatherForecast")
.Produces<WeatherForecast[]>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError);

app.Run();

// WeatherForecast record
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Service Interface
public interface I_WeatherService
{
    Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync(string[] summaries);
}

// Service Implementation
public class WeatherService : I_WeatherService
{
    public Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync(string[] summaries)
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            )).ToArray();

        return Task.FromResult<IEnumerable<WeatherForecast>>(forecast);
    }
}
