using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniChatApp2.Model;

namespace MiniChatApp2.Interfaces
{
    public interface IUserService
    {
       
        Task<User> GetUserByEmail(string email);

        Task<LoginResponseDto> LoginAsync(LoginDto model);

        Task<User> RegisterUserAsync(User user);
    }
}