using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Controllers.Extentions
{
    public static class ControllerExtentions
    {
        public static string GenerateJwtToken(this IEnumerable<Claim> claims, HashSet<string> validApiKeys)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(validApiKeys.First()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "Gateway",
                audience: "Microservices",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
