using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;

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
