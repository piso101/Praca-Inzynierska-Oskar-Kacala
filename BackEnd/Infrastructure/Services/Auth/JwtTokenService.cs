using BackEnd.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackEnd.Infrastructure.Services.Auth
{
    public class JwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(UserAccount user)
        {
            var jwtKey = _config["Jwt:Key"];
            var jwtIssuer = _config["Jwt:Issuer"] ?? "app";
            var jwtAudience = _config["Jwt:Audience"] ?? "app";

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.DisplayName ?? user.Email ?? "user"),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var jwtKey = _config["Jwt:Key"];
            var jwtIssuer = _config["Jwt:Issuer"] ?? "app";
            var jwtAudience = _config["Jwt:Audience"] ?? "app";

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = key,
                ValidateLifetime = true, // sprawdza czy token nie wygas³
                ClockSkew = TimeSpan.Zero // brak "okna tolerancji" czasu
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception)
            {
                return null; // token niepoprawny lub wygas³
            }
        }
    }
}