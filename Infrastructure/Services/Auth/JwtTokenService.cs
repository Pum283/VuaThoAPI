using Domain.Entity;
using Domain.Enum;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services.Auth
{
    public interface IJwtTokenService
    {
        string CreateAccessToken(User user, string role, WorkerType? workerType, WorkerStatus? workerStatus);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateAccessToken(User user, string role, WorkerType? workerType, WorkerStatus? workerStatus)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var issuer = jwtSection["Issuer"] ?? "VuaTho";
            var audience = jwtSection["Audience"] ?? "VuaTho";
            var signingKey = jwtSection["SigningKey"];
            var minutes = int.TryParse(jwtSection["AccessTokenMinutes"], out var m) ? m : 60;

            if (string.IsNullOrWhiteSpace(signingKey))
            {
                throw new InvalidOperationException("Thiếu cấu hình JWT signing key. Vui lòng thiết lập biến môi trường 'Jwt__SigningKey'.");
            }

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new("role", role),
                new(ClaimTypes.Role, role),
            };

            if (!string.IsNullOrWhiteSpace(user.FullName))
            {
                claims.Add(new("fullName", user.FullName));
                claims.Add(new(ClaimTypes.Name, user.FullName));
            }

            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                claims.Add(new("phoneNumber", user.PhoneNumber));
            }

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(new(JwtRegisteredClaimNames.Email, user.Email));
            }

            if (workerType.HasValue)
            {
                claims.Add(new("workerType", workerType.Value.ToString()));
            }

            if (workerStatus.HasValue)
            {
                claims.Add(new("workerStatus", workerStatus.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

