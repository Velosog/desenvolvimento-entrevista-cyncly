using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ErpDemo.Application.DTOs;
using ErpDemo.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ErpDemo.Application.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    private static readonly Dictionary<string, (string Password, string Role)> Users = new()
    {
        ["admin@demo.com"] = ("123456", "Admin"),
        ["operator@demo.com"] = ("123456", "Operator")
    };

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        if (!Users.TryGetValue(request.Email.ToLower(), out var user) || user.Password != request.Password)
            throw new UnauthorizedAccessException("Email ou senha inválidos.");

        var expiresAt = DateTime.UtcNow.AddHours(8);
        var token = GenerateToken(request.Email, user.Role, expiresAt);

        return Task.FromResult(new LoginResponseDto
        {
            Token = token,
            Email = request.Email,
            Role = user.Role,
            ExpiresAt = expiresAt
        });
    }

    private string GenerateToken(string email, string role, DateTime expiresAt)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "SuperSecretKeyForDemoErpApplication2024!@#$%";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "ErpDemo",
            audience: _configuration["Jwt:Audience"] ?? "ErpDemoApp",
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
