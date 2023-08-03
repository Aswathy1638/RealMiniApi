using MiniChatApp2.Model;

namespace MiniChatApp2.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(UserRegistrationDto model);
        Task<User> GetUserByEmail(string email);
      
    }
}