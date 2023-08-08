using Microsoft.AspNetCore.Identity;
using MiniChatApp2.Model;

namespace MiniChatApp2.Interfaces
{
    public interface IUserRepository
    {

        Task<IdentityUser<int>> FindByEmailAsync(string email);
        Task<IdentityResult> CreateAsync(IdentityUser<int> user, string password);


        //  Task<User> AddUserAsync(User user);
        // Task<User> GetUserByEmailAsync(string email);
        //Task SaveChangesAsync();
        //Task<List<User>> GetAllUsersAsync();
    }
}