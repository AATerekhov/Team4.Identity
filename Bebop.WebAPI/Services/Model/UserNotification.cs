namespace WebAPI.Services.Model
{
    public record UserNotification
    {

        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        public UserNotification(string userId, string email, string name)
        {
            UserId = userId;
            Email = email;
            Name = name;
        }
        public override int GetHashCode()
        {
            return Email.GetHashCode();
        }
    }
}
