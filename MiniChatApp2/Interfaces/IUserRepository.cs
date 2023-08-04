using MiniChatApp2.Model;

namespace MiniChatApp2.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<User> GetUserByEmailAsync(string email);
        Task SaveChangesAsync();
        Task<List<User>> GetAllUsersAsync();
    }
}