namespace OrderManagementApi.Models.DTOs;

/// <summary>
/// Giriş yaparken alınacak kullanıcı adı ve şifre verisi.
/// </summary>
public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
