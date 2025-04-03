using WebAPI.Services.Model;

namespace WebAPI.Services.Repositories
{
    public interface IUserRepository
    {
       UserNotification GetByEmail(string email);
        UserNotification Create(UserNotification user);
    }
}
