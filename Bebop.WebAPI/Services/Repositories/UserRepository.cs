using System.Collections.Concurrent;
using WebAPI.Services.Model;

namespace WebAPI.Services.Repositories
{
    public class UserRepository : IUserRepository
    {
        private static ConcurrentDictionary<string, UserNotification> _userManager = new ConcurrentDictionary<string, UserNotification>();

        public UserNotification Create(UserNotification user)
        {
            if (!_userManager.ContainsKey(user.Email))
            {
                _userManager.TryAdd(user.Email, user);
                return user;
            }
            else return user;
        }

        public UserNotification GetByEmail(string email)
        {
            _userManager.TryGetValue(email, out UserNotification userNotification);
            return userNotification;
        }
    }
}
