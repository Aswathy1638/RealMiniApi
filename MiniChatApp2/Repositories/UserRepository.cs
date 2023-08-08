using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniChatApp2.Data;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Model;

namespace MiniChatApp2.Repositories
{
    public class UserRepository:IUserRepository
    {

        private readonly UserManager<IdentityUser<int>> _userManager;
        public UserRepository(UserManager<IdentityUser<int>> userManager)
        {
            _userManager = userManager;
        }

        public Task<IdentityUser<int>> FindByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }
        public Task<IdentityResult> CreateAsync(IdentityUser<int> user, string password)
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
        //public async Task<User> AddUserAsync(User user)
        //  {
        //      var result = await _userManager.CreateAsync(user, user.Password);

        //      if (result.Succeeded)
        //      {
        //          return user;
        //      }

        //      return null;
        //  }


        /* public async Task<User> GetUserByEmailAsync(string email)
         {
             return await _context.User.FirstOrDefaultAsync(u => u.Email == email);
         }
         public async Task SaveChangesAsync()
         {
             await _context.SaveChangesAsync();
         }
         public async Task<List<User>> GetAllUsersAsync()
         {
             return await _context.User.ToListAsync();
         }*/
    }
}
