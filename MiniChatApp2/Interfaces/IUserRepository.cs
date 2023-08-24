using Microsoft.AspNetCore.Identity;
using MiniChatApp2.Model;

namespace MiniChatApp2.Interfaces
{
    public interface IUserRepository
    {

        Task<IdentityUser> FindByEmailAsync(string email);
        Task<IdentityResult> CreateAsync(IdentityUser user, string password);
        Task<List<UserProfile>> GetAllUsersAsync(string currentUserEmail);
       
    }
}