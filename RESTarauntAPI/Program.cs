using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RESTarauntAPI.Data;
using RESTarauntAPI.Services;

namespace RESTarauntAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization(); //Сервис Авторизации (предоставления прав)   
        
        // Регистрация DbContext с подключением к базе данных
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddScoped<UserService>(); // Регистрируем сервис для работы с пользователями
        builder.Services.AddScoped<OrderService>();
        builder.Services.AddScoped<MenuService>();
        builder.Services.AddScoped<CookingBotService>();
        
        builder.Services.AddControllers();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        // Добавляем Swagger с поддержкой JWT авторизации
        builder.Services.AddSwaggerGen(options =>
        {
            // Настройка для использования JWT в Swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Введите JWT токен в поле 'Bearer {токен}'."
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
                    new string[] {}
                }
            });
        });

        // Схема аутентификации (входа) - с помощью jwt-токенов
        builder.Services.AddAuthentication("Bearer")  
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
        
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.UseAuthentication();   // добавление middleware аутентификации 
        app.UseAuthorization();   // добавление middleware авторизации 
        
        app.MapControllers();
        
        app.Run();
    }
    
}