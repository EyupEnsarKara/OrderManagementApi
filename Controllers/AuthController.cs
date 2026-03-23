using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrderManagementApi.Models.DTOs;

namespace OrderManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Sistemde sahte bir Admin yetkisi veren giriş metodu.
    /// Gerçek bir kullanıcı tablosu veya karmaşık Identity olmadan direkt Token döndürür.
    /// Kullanıcı Adı: admin, Şifre: 123456
    /// </summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        // Temel, güvenliksiz ama pratik Login Simulasyonu
        if (loginDto.Username != "admin" || loginDto.Password != "123456")
        {
            return Unauthorized(new { message = "Kullanıcı adı veya şifre hatalı!" });
        }

        // --- Token Üretme Aşaması ---
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Sisteme bu kişinin "Admin" rolünde olduğunu söylüyoruz
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, loginDto.Username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var expireMinutes = Convert.ToInt32(_config["Jwt:ExpireMinutes"]);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(expireMinutes),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { token = tokenString });
    }
}
