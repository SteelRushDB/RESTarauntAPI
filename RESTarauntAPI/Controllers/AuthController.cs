using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RESTarauntAPI.Models;
using RESTarauntAPI.Services;

namespace RESTarauntAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IConfiguration _configuration;
    
    public AuthController(UserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    
    
    [HttpPost("register")] // HTTP-метод POST для регистрации
    public async Task<IActionResult> Register([FromBody] UserRegistrationDTO registrationDto)
    {
        if (!_userService.IsEmailValid(registrationDto.Email))
        {
            return BadRequest("Email address must contain '@'."); // Проверка, что email содержит '@'
        }
        if (_userService.IsEmailTaken(registrationDto.Email))
        {
            return BadRequest("Email is already taken."); // Проверка, есть ли такой email
        }

        var user = await _userService.RegisterUserAsync(registrationDto); // Регистрация пользователя
        return Ok(new 
        {
            Message = "Registration successful!", // Подтверждающее сообщение
            User = new 
            {
                user.Id,
                user.Username,
                user.Email,
                user.Role
            }
        });
    }
    
    
    

    [HttpPost("login")] // HTTP-метод POST для авторизации
    public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
    {
        var user = await _userService.GetUserByEmailAsync(loginDto.Email);

        if (user == null || !_userService.VerifyPassword(user, loginDto.Password))
        {
            return Unauthorized("Invalid email or password.");
        }
        
        var token = GenerateJwtTokenAsync(user);
        return Ok(new
        {
            Message = "Login successful!", // Подтверждающее сообщение
            token
        });
    }

    private async Task<string> GenerateJwtTokenAsync(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role) // Добавляем роль пользователя в токен
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials);

        // Сериализация токена в строку
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        
        await _userService.AddTokenInDbAsync(user, tokenString, token);
        
        return tokenString;
    }
    
    
    
    [Authorize] // Требуем авторизацию через JWT
    [HttpGet("info")]
    public IActionResult GetUserInfo()
    {
        // Извлекаем клеймы из токена
        var userClaims = User.Claims;
        
        // Получаем email и роль из клеймов
        var email = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value; // Это мы поместили в JwtRegisteredClaimNames.Sub
        if (email == null)
        {
            return Unauthorized(new { message = "Token is invalid or expired" });
        }
        
        var role = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value; // Это клейм роли, добавленный при генерации токена
        if (role == null)
        {
            return Unauthorized(new { message = "Role not found in token" });
        }

        

        // Возвращаем информацию о пользователе
        return Ok(new
        {
            Email = email,
            Role = role
        });
    }
    
}