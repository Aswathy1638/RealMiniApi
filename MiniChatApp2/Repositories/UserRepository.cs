using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniChatApp2.Data;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;

namespace MiniChatApp2.Repositories
{
    public class UserRepository: IUserRepository
    {

        private readonly UserManager<IdentityUser> _userManager;
        public UserRepository(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<IdentityUser> FindByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }
        public Task<IdentityResult> CreateAsync(IdentityUser user, string password)
        {
            return _userManager.CreateAsync(user, password);
        }
        public async Task<List<UserProfile>> GetAllUsersAsync(string currentUserEmail)
        {
            var allUsers = await _userManager.Users.ToListAsync();

            // Exclude the current user from the list
            var users = allUsers.Where(u => u.Email != currentUserEmail).Select(u => new UserProfile
            {
                Id = u.Id,
                Name = u.UserName,
                Email = u.Email
            }).ToList();

            return users;
        }
        
    }
}
