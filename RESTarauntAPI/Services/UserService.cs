using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RESTarauntAPI.Data;
using RESTarauntAPI.Models;

namespace RESTarauntAPI.Services;

public class UserService
{
    private readonly AppDbContext _context;
    
    public UserService(AppDbContext context)
    {
        _context = context;
    }
    
    // Регистрация нового пользователя
    public async Task<User> RegisterUserAsync(UserRegistrationDTO dto)
    {
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), // Хешируем пароль
            Role = dto.Role 
        };

        _context.Users.Add(user); // Добавляем пользователя в базу
        await _context.SaveChangesAsync(); // Сохраняем изменения
        return user;
    }

    // Проверка на существование почты
    public bool IsEmailTaken(string email)
    {
        return _context.Users.Any(u => u.Email == email);
    }
    
    // Проверка на то что это почта
    public bool IsEmailValid(string email)
    {
        return email.Contains('@');
    }
    
    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
    }

    public bool VerifyPassword(User user, string loginDtoPassword)
    {
        return BCrypt.Net.BCrypt.Verify(loginDtoPassword, user.PasswordHash);
    }




    public async Task AddTokenInDbAsync(User user, string tokenString, JwtSecurityToken token)
    {
        var session = new Session
        {
            UserId = user.Id,
            SessionToken = tokenString,
            ExpiresAt = token.ValidTo
        };
        _context.Sessions.Add(session);
        await _context.SaveChangesAsync(); // Сохраняем изменения
    }


    public bool IsRoleValid(string role)
    {
        if (role == "Admin" || role == "Manager" || role == "Employee" || role == "User") return true;
        return false;
    }
}