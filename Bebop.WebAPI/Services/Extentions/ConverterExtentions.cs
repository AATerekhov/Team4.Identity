using System.Security.Claims;
using WebAPI.Services.Model;

namespace WebAPI.Services.Extentions
{
    public static class ConverterExtentions
    {
        public static UserNotification Convert(this ClaimsPrincipal claims) 
        {
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userMail = claims.FindFirst(ClaimTypes.Email)?.Value;
            var name = claims.FindFirst(ClaimTypes.GivenName)?.Value;
            return new UserNotification(userId, userMail, name);
        }
    }
}
