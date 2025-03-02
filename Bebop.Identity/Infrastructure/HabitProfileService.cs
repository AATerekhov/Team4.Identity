using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Infrastructure
{
    public class HabitProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var emailClaim = context.Subject.FindFirst(JwtClaimTypes.Email);
            var nameClaim = context.Subject.FindFirst(JwtClaimTypes.GivenName);

            if (emailClaim != null && nameClaim != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, emailClaim?.Value),
                    new Claim(ClaimTypes.GivenName, nameClaim?.Value)
                };
                context.IssuedClaims.AddRange(claims);
            }
            else
            {
                emailClaim = context.Subject.FindFirst(ClaimTypes.Email);
                nameClaim = context.Subject.FindFirst(ClaimTypes.GivenName);

                var claims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Email, emailClaim?.Value),
                    new Claim(JwtClaimTypes.GivenName, nameClaim?.Value)
                };
                context.IssuedClaims.AddRange(claims);
            }
            
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}
