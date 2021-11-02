using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VendingMachine.Services.Application.Abstractions;
using VendingMachine.Services.Application.Models;

namespace VendingMachine.Services.Application
{
    public class TokenService : ITokenService
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationInMinutes;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration configuration)
        {
            _issuer = configuration.GetSection("Token")["Issuer"];
            _audience = configuration.GetSection("Token")["Audience"];
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_issuer));

            int.TryParse(configuration.GetSection("Token")["ExpirationInMinutes"], out _expirationInMinutes);
        }

        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("UserId", user.Id.ToString())
            };

            var tokenOptions = new JwtSecurityToken(issuer: _issuer,
                                                    audience: _audience,
                                                    claims: claims,
                                                    expires: DateTime.Now.AddMinutes(_expirationInMinutes),
                                                    signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}
