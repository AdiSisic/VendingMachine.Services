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
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration configuration)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Token")["Issuer"]));
        }

        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            { 
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var tokenOptions = new JwtSecurityToken(issuer: "http://localhost:30725",
                                                    audience: "http://localhost:30725",
                                                    claims: claims,
                                                    expires: DateTime.Now.AddMinutes(5),
                                                    signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}
