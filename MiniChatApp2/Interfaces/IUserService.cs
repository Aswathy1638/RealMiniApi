using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniChatApp2.Model;
using NuGet.Protocol.Plugins;

namespace MiniChatApp2.Interfaces
{
    public interface IUserService
    {
        Task<(bool success, object result)> RegisterUserAsync(UserRegistration request);
        Task<LoginResponseDto> VerifyTokenAsync(string tokenId);
        Task<LoginResponseDto> LoginAsync(LoginDto model);
        Task<List<UserProfile>> GetAllUsersAsync(string currentUserEmail);

    }
}