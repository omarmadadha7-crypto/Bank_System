using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Business_Logic.Interfaces;
using Business_Logic.Settings;
using Data_Access.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Business_Logic.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;

        public TokenService(JwtSettings settings)
        {
            _settings = settings;
        }

        public (string Token, DateTime ExpiresAt) CreateToken(User user, string roleName)
        {
            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(_settings.ExpiryMinutes);

            // Short claim names on purpose: they match NameClaimType / RoleClaimType configured in the API
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("name", user.UserName),
                new Claim("role", roleName),
                new Claim("branch_id", user.BranchId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: now,
                expires: expiresAt,
                signingCredentials: credentials);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }
    }
}
